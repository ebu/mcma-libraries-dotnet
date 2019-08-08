dotnet pack base/Mcma.Core -p:PackageVersion=%1
dotnet nuget push base/Mcma.Core/bin/packages/Mcma.Core.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json
dotnet pack base/Mcma.Client -p:PackageVersion=%1
dotnet nuget push base/Mcma.Client/bin/packages/Mcma.Client.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json
dotnet pack base/Mcma.Data -p:PackageVersion=%1
dotnet nuget push base/Mcma.Data/bin/packages/Mcma.Data.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json
dotnet pack base/Mcma.Worker -p:PackageVersion=%1
dotnet nuget push base/Mcma.Worker/bin/packages/Mcma.Worker.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json
dotnet pack base/Mcma.Api -p:PackageVersion=%1
dotnet nuget push base/Mcma.Api/bin/packages/Mcma.Api.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json

dotnet pack aws/Mcma.Aws.Client -p:PackageVersion=%1
dotnet nuget push aws/Mcma.Aws.Client/bin/packages/Mcma.Aws.Client.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json
dotnet pack aws/Mcma.Aws.DynamoDb -p:PackageVersion=%1
dotnet nuget push aws/Mcma.Aws.DynamoDb/bin/packages/Mcma.Aws.DynamoDb.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json
dotnet pack aws/Mcma.Aws.Lambda -p:PackageVersion=%1
dotnet nuget push aws/Mcma.Aws.Lambda/bin/packages/Mcma.Aws.Lambda.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json
dotnet pack aws/Mcma.Aws.S3 -p:PackageVersion=%1
dotnet nuget push aws/Mcma.Aws.S3/bin/packages/Mcma.Aws.S3.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json
dotnet pack aws/Mcma.Aws.ApiGateway -p:PackageVersion=%1
dotnet nuget push aws/Mcma.Aws.ApiGateway/bin/packages/Mcma.Aws.ApiGateway.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json
dotnet pack aws/Mcma.Aws.LambdaWorkerInvoker -p:PackageVersion=%1
dotnet nuget push aws/Mcma.Aws.LambdaWorkerInvoker/bin/packages/Mcma.Aws.LambdaWorkerInvoker.%1.nupkg -k=%NUGET_API_KEY% -s https://api.nuget.org/v3/index.json