terraform {
  required_version = "~> 1.2.0"

  required_providers {
    rabbitmq = {
        source = "cyrilgdn/rabbitmq"
        version = "1.6.0"
    }
  }
}

provider "rabbitmq" {
  endpoint = "http://localhost:15672"
  username = "guest"
  password = "guest"
}

resource "rabbitmq_vhost" "example_vhost" {
  name = "example_vhost"
}
