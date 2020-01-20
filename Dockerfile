FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
COPY CodeRedCryptoAPI/app/publish .
ENTRYPOINT ["dotnet", "CodeRedCryptoAPI.dll"]
EXPOSE 10800

