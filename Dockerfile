FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
COPY WebApplication4/app/publish .
ENTRYPOINT ["dotnet", "CodeRedCryptoAPI.dll"]
EXPOSE 10800

