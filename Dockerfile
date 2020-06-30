FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine
WORKDIR /app
COPY /app .
ENTRYPOINT ["dotnet", "issue-client-cert.dll"]
