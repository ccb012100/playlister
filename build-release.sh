#!/usr/bin/env bash
dotnet publish --self-contained true --runtime linux-x64 -c Release -p:PublishReadyToRun=true
