FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore Broker.Face.Http/Broker.Face.Http.csproj
COPY . .
RUN dotnet build Broker.Face.Http/Broker.Face.Http.csproj -c Release -o /app/build
RUN dotnet publish Broker.Face.Http/Broker.Face.Http.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Broker.Face.Http.dll"]
