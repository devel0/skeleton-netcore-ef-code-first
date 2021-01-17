#!/bin/bash

executing_dir()
{
        dirname `readlink -f "$0"`
}
exdir=$(executing_dir)

cd "$exdir"
dotnet run --backup-migrations "$exdir"/Migrations

