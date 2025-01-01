FROM docker.elastic.co/logstash/logstash:8.14.3

COPY ./drivers/ /usr/share/logstash/logstash-core/lib/jars/

COPY config/ /usr/share/logstash/config/

CMD ["logstash", "-f", "/usr/share/logstash/config/users_sync.conf"]