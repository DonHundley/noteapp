FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

WORKDIR /src

COPY . .
RUN dotnet restore "api/api.csproj"
RUN dotnet build "api/api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api/api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 8181
EXPOSE 5000
ENTRYPOINT ["dotnet", "api.dll"]