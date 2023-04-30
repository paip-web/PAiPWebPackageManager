#!/usr/bin/env pwsh
npx nodemon -e "cs,sh,bat,ps1,yaml,yml,Dockerfile,dockerignore" -w .. -V -I -x "docker-compose run --rm --build app-test"