#!/bin/bash

DIR="$(cd "$(dirname "$0")" && pwd)"
cd $DIR/../docker

docker exec -it shino_php_fpm /bin/bash