#!/bin/bash

HOST_DOMAIN="host.docker.internal"
HOST_IP=$(ip route | awk 'NR==1 {print $3}')
echo -e "$HOST_IP\t$HOST_DOMAIN" >> /etc/hosts

exec "$@"