FROM mcr.microsoft.com/dotnet/core/runtime:3.1
RUN sed -i "s|DEFAULT@SECLEVEL=2|DEFAULT@SECLEVEL=1|g" /etc/ssl/openssl.cnf
WORKDIR /app
COPY /app .
ENTRYPOINT ["dotnet", "issue-client-cert.dll"]
