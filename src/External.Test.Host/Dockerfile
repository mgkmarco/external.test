FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://++:8080
EXPOSE 8080

FROM registry.access.redhat.com/ubi8-minimal AS runtime
RUN microdnf install libicu glibc-devel
WORKDIR /opt/app
ENV InfoManagement__Checksum__Root=/opt/app/checksum/
ENV ASPNETCORE_URLS=http://++:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ENV COMPlus_ReadyToRun=0
WORKDIR /sln
ONBUILD COPY ./*.sln  ./NuGet.Config ./
ONBUILD ARG BUILD_VERSION
# If BUILD_VERSION is set/non-empty, use it, otherwise use a default value
ONBUILD ARG VERSION=${BUILD_VERSION:-1.0.0}
COPY ["src/External.Test.Host/External.Test.Host.csproj", "src/External.Test.Host/"]
RUN dotnet restore "src/External.Test.Host/External.Test.Host.csproj"
COPY . .
ARG VERSION=1.0.0
ARG INFORMATIONAL_VERSION=$VERSION
RUN dotnet build /p:Version=$VERSION /p:InformationalVersion=$INFORMATIONAL_VERSION -c Release
RUN dotnet build "src/External.Test.Host/External.Test.Host.csproj" /p:Version=$VERSION /p:InformationalVersion=$INFORMATIONAL_VERSION -c Release -r linux-x64 /m:1

FROM build AS publish
WORKDIR "/sln/src/External.Test.Host"
RUN dotnet publish "External.Test.Host.csproj" -c Release -o /app/publish --no-build -r linux-x64
WORKDIR "/app/publish"

FROM runtime AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["./External.Test.Host"]