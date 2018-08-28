# Introduction 
This repository contains a sample ASP.NET Core Web API project as well as a Test project. The purpose of this solution is to demonstrate the ability to test an ASP.NET Core application relying on a Cosmos DB back-end, using the emulator in a VSTS Build CI/CD pipeline.  

# Getting Started
Here are the steps to reuse this sample and build a Build pipeline to test the ASP.NET Core Web API project.

1.	Clone the repository locally
```
git clone https://github.com/joudot/CosmosDB-EmulatorTesting-VSTS
```
2.	Navigate into the project and push it to VSTS
```
cd CosmosDB-EmulatorTesting-VSTS/

git remote set-url origin https://<YOUR_VSTS_TENANT>.visualstudio.com/<YOUR_PROJECT>/_git/<YOUR_REPOSITORY>

git push -u origin --all
```
