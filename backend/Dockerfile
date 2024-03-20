# Use the .NET SDK image as a base for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

# Set the working directory in the container
WORKDIR /src

# Copy the entire application to the container
COPY . .

# Restore NuGet packages
RUN dotnet restore "api/api.csproj"

# Build the application
RUN dotnet build "api/api.csproj" -c Release -o /app/build

# Intermediate stage for publishing
FROM build AS publish

# Publish the application
RUN dotnet publish "api/api.csproj" -c Release -o /app/publish

# Use the .NET runtime image as the base for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory in the container
WORKDIR /app

# Copy the published application to the container
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 8181
EXPOSE 5000

# Install gstreamer
RUN apt-get update && apt-get install -y gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly

# Set the entry point for the container
ENTRYPOINT ["dotnet", "api.dll"]