#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["II_PROJEKAT.csproj", "."]
RUN dotnet restore "./II_PROJEKAT.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "II_PROJEKAT.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "II_PROJEKAT.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "II_PROJEKAT.dll"]