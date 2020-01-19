FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 10800
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /home
COPY ["CodeRedCryptoAPI/CodeRedCryptoAPI.csproj", "CodeRedCryptoAPI/"]
# ADD $artifactory_nuget_config /home
# RUN dotnet restore "CodeRedCryptoAPI/CodeRedCryptoAPI.csproj" --configfile /home/nuget.config
COPY . .
WORKDIR /home/CodeRedCryptoAPI
RUN dotnet build "CodeRedCryptoAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CodeRedCryptoAPI.csproj" -c Release -o /app

RUN dotnet dev-certs https -ep /app/aspnetapp.pfx -p 12345678

# RUN rm /home/nuget.config

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENV DOTNET_ENVIRONMENT container
ENTRYPOINT ["dotnet", "CodeRedCryptoAPI.dll"]
