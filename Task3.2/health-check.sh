#!/bin/bash

echo "Health checking..."
curl -f http://localhost:8080/api/health || exit 1
echo "All services are healthy!"
