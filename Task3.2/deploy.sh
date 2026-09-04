#!/bin/bash
set -e

echo "Deploying..."
git pull origin main
docker compose -f compose.yaml build --no-cache
docker compose -f compose.yaml up -d
docker compose -f compose.yaml exec api dotnet ef database update -s Api
echo "Deployed!"


