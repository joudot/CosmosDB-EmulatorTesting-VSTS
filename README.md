# Introduction 
This repository contains a sample ASP.NET Core Web API project as well as a Test project. The purpose of this solution is to demonstrate the ability to test an ASP.NET Core application relying on a Cosmos DB back end using the emulator in a VSTS environment.  

# Getting Started
Here are the step to reuse this sample to build a Build pipeline to test this ASP.NET Core Web API.

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
