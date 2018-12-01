FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY src/Sodao.GrpcExampleCrossPlat.Service/Sodao.GrpcExampleCrossPlat.Service.csproj src/Sodao.GrpcExampleCrossPlat.Service/
COPY src/Sodao.GrpcExample.Application/Sodao.GrpcExample.Application.csproj src/Sodao.GrpcExample.Application/
COPY src/Sodao.GrpcExample.Domain/Sodao.GrpcExample.Domain.csproj src/Sodao.GrpcExample.Domain/
COPY src/Sodao.GrpcExampleCrossPlat.Service.Grpc/Sodao.GrpcExampleCrossPlat.Service.Grpc.csproj src/Sodao.GrpcExampleCrossPlat.Service.Grpc/
COPY src/Sodao.Core.Grpc/Sodao.Core.Grpc.csproj src/Sodao.Core.Grpc/
RUN dotnet restore
COPY . .
WORKDIR /src/src/Sodao.GrpcExampleCrossPlat.Service
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sodao.GrpcExampleCrossPlat.Service.dll"]
