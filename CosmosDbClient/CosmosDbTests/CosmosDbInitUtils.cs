using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace CosmosDbTests
{
    public static class CosmosDbInitUtils
    {
        public static async Task RunBulkImportAsync(DocumentClient client, string collectionLink)
        {
            try
            {
                string inputDirectory = @".\Data\";
                string inputFileMask = "*.json";
                int maxFiles = 2000;
                int maxScriptSize = 50000;

                // 1. Get the files.
                string[] fileNames = Directory.GetFiles(inputDirectory, inputFileMask);
                DirectoryInfo di = new DirectoryInfo(inputDirectory);
                FileInfo[] fileInfos = di.GetFiles(inputFileMask);

                // 2. Prepare for import.
                int currentCount = 0;
                int fileCount = maxFiles != 0 ? Math.Min(maxFiles, fileNames.Length) : fileNames.Length;

                // 3. Create stored procedure for this script.
                string body = File.ReadAllText(@".\JS\BulkImport.js");
                StoredProcedure sproc = new StoredProcedure
                {
                    Id = "BulkImport",
                    Body = body
                };
                Console.WriteLine("Deleting SProc");
                await TryDeleteStoredProcedure(client, collectionLink, sproc.Id);
                Console.WriteLine("Creating Sproc");
                sproc = await client.CreateStoredProcedureAsync(collectionLink, sproc);
                Console.WriteLine("Adding Files");
                Stopwatch sp = new Stopwatch();
                sp.Start();
                // 4. Create a batch of docs (MAX is limited by request size (2M) and to script for execution.           
                // We send batches of documents to create to script.
                // Each batch size is determined by MaxScriptSize.
                // MaxScriptSize should be so that:
                // -- it fits into one request (MAX reqest size is 16Kb).
                // -- it doesn't cause the script to time out.
                // -- it is possible to experiment with MaxScriptSize to get best perf given number of throttles, etc.
                while (currentCount < fileCount)
                {
                    // 5. Create args for current batch.
                    //    Note that we could send a string with serialized JSON and JSON.parse it on the script side,
                    //    but that would cause script to run longer. Since script has timeout, unload the script as much
                    //    as we can and do the parsing by client and framework. The script will get JavaScript objects.
                    string argsJson = CreateBulkInsertScriptArguments(fileNames, currentCount, fileCount, maxScriptSize);
                    var args = new dynamic[] { JsonConvert.DeserializeObject<dynamic>(argsJson) };

                    // 6. execute the batch.
                    StoredProcedureResponse<int> scriptResult = await client.ExecuteStoredProcedureAsync<int>(
                        sproc.SelfLink,
                        new RequestOptions(),
                        args);

                    // 7. Prepare for next batch.
                    int currentlyInserted = scriptResult.Response;
                    currentCount += currentlyInserted;
                }
                Console.WriteLine("Files added");
                // 8. Validate
                int numDocs = 0;
                string continuation = string.Empty;
                do
                {
                    // Read document feed and count the number of documents.
                    FeedResponse<dynamic> response = await client.ReadDocumentFeedAsync(collectionLink, new FeedOptions { RequestContinuation = continuation });
                    numDocs += response.Count;

                    // Get the continuation so that we know when to stop.
                    continuation = response.ResponseContinuation;
                }
                while (!string.IsNullOrEmpty(continuation));

                Console.WriteLine("Found {0} documents in the collection inserted in {1}ms\r\n", numDocs, sp.Elapsed.Milliseconds);
            }
            catch (DocumentClientException e)
            {
                // Bad request is returned when trying to insert existing documents
                if (e.StatusCode != HttpStatusCode.BadRequest)
                {
                    Console.WriteLine($"DocumentClientException caught in CosmosDbInitUtils {e.Message}");
                    throw e;
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Exception caught in CosmosDbInitUtils {e.Message}");
                throw e;
            }
        }

        private static async Task TryDeleteStoredProcedure(DocumentClient client, string collectionLink, string sprocId)
        {
            StoredProcedure sproc = client.CreateStoredProcedureQuery(collectionLink).Where(s => s.Id == sprocId).AsEnumerable().FirstOrDefault();
            if (sproc != null)
            {
                await client.DeleteStoredProcedureAsync(sproc.SelfLink);
            }
        }

        private static string CreateBulkInsertScriptArguments(string[] docFileNames, int currentIndex, int maxCount, int maxScriptSize)
        {
            var jsonDocumentArray = new StringBuilder();
            jsonDocumentArray.Append("[");

            if (currentIndex >= maxCount) return string.Empty;
            jsonDocumentArray.Append(File.ReadAllText(docFileNames[currentIndex]));

            int scriptCapacityRemaining = maxScriptSize;
            string separator = string.Empty;

            int i = 1;
            while (jsonDocumentArray.Length < scriptCapacityRemaining && (currentIndex + i) < maxCount)
            {
                jsonDocumentArray.Append(", " + File.ReadAllText(docFileNames[currentIndex + i]));
                i++;
            }

            jsonDocumentArray.Append("]");
            return jsonDocumentArray.ToString();
        }
    }
}
