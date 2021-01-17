#!/bin/bash

exdir=$(dirname `readlink -f "$0"`)

if [ ! -e /opt/schemacrawler/_schemacrawler/schemacrawler.sh ]; then
	echo "can't find schemacrawler"
	echo "see https://github.com/devel0/knowledge/blob/4275a7062f0126631d961afbd0c9d5b5154002d7/doc/psql-schema-crawler.md"
	exit 1
fi

if [ ! -e ~/.pgpass ]; then
	echo "can't find ~/.pgpass"
	exit 1
fi

# set your db host
dbhost=localhost
dbname=srvdb

pgpass="$(cat ~/.pgpass | grep $dbhost | awk -F ':' '{print $5}')"

if [ "$pgpass" == "" ]; then
	echo "can't find ~/.pgpass pass for dbhost=[$dbhost]"
	exit 1
fi

cd "$exdir"/doc/db

SC_GRAPHVIZ_OPTS='-Granksep=1.5' /opt/schemacrawler/_schemacrawler/schemacrawler.sh \
    --log-level=SEVERE \
    --portable-names \
    --sort-columns \
    --exclude-columns='public.(__EFMigrationsHistory).*' \
    --server=postgresql \
    --command=schema \
    --host=$dbhost \
    --user=postgres \
    --password="$pgpass" \
    --database="$dbname" \
    -o=db.png \
    --outputformat=png \
    --info-level=standard

cd "$exdir"
