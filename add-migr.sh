#!/bin/bash

executing_dir()
{
	dirname `readlink -f "$0"`
}
exdir=$(executing_dir)

cd "$exdir"
#migrname="migr-$(uuid -F siv)"
migrname="migr-$(date -u -Is)"

echo "---> creating migration [$migrname]"
dotnet ef migrations add $migrname

if [ "$?" != "0" ]; then
	echo "*** err"
    echo "may need to install tool try with before:"
    echo "dotnet tool install --global dotnet-ef"
	exit 1
fi

echo "---> updating database"
dotnet ef database update

if [ "$?" != "0" ]; then
	echo "*** skip backup migrations"
	echo "*** removing failed db update migration"
        dotnet ef migrations remove
else
	echo "---> backup migrations"
	"$exdir"/backup-migr.sh

	cd "$exdir"

	echo "If you working on a shared db is suggested to commit/push your sources now so that other developers can restore migrations from database with accordingly code-first sources"
fi
