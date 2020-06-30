# HTTPS issue for .NET Core on Linux

This repository contains a repro for the issue [Initial HTTPS request with client certificate fails on Linux, subsequent requests succeeds](https://github.com/dotnet/runtime/issues/37952).

## Steps to reproduce

The issue does not occure when running Windows, but can be reproduced in a Docker container

1. Run `.\build.ps1`, this will create a docker image `issue-client-cert`
2. Run the newly built container with `docker run --rm  issue-client-cert`

Expected output:

```
> docker run --rm  issue-client-cert
Making an HTTPS call to Google without client certificate.
Loading certificate from /app/swish_client_certificate_test.p12.
Certificate chain consists of 3 entries.
The first HTTP request failed with 'sslv3 alert handshake failure'.
Waiting for 2 seconds, a direct call will fail again.
The second HTTP request succeeded.
```