#!/bin/bash
for i in "BetterHealthChecks.Core" "BetterHealthChecks.Http" "BetterHealthChecks.Ignite" "BetterHealthChecks.Kafka" "BetterHealthChecks.PostgreSQL"
do
  project="./src/$i/$i.csproj"
  echo -e "restore ./src/$i/$i.csproj"
  dotnet restore ./src/$i/$i.csproj
  dotnet build $project -c Release /p:Version="$1"
  dotnet pack $project -o ./packs/$1/ /p:Version=$1
done