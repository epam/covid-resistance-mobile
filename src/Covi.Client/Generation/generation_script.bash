#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

allJsons=$DIR/"*.json"

for f in $allJsons; do

name=$(basename "$f" .json)

rm "$(dirname $DIR)"/"Services/""$name"/*.cs
rm "$(dirname $DIR)"/"Services/""$name"/Models/*.cs

autorest --v3 --input-file="$f" --csharp --use-internal-constructors --output-folder="$(dirname $DIR)"/"Services/""$name" --namespace=Covi.Client.Services.$name

done
