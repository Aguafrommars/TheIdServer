provider "kubernetes" {
  host = var.k8s_config.host
  token = var.k8s_config.token
  client_certificate = base64decode(var.k8s_config.client_certificate)
  client_key = base64decode(var.k8s_config.client_key)
  cluster_ca_certificate = base64decode(var.k8s_config.cluster_ca_certificate)
}

resource "kubernetes_config_map" "mysql_master_config" {
  metadata {
    name = "mysql-master-config"
    namespace = "theidserver"
  }

  data = {
    "my.cnf" = file("${path.module}/my-master.cnf")
  }
}

resource "kubernetes_config_map" "mysql_scondary_config" {
  metadata {
    name = "mysql-secondary-config"
    namespace = "theidserver"
  }

  data = {
    "my.cnf" = file("${path.module}/my-secondary.cnf")
  }
}

provider "helm" {
  kubernetes {
    host = var.k8s_config.host
    token = var.k8s_config.token
    client_certificate = base64decode(var.k8s_config.client_certificate)
    client_key = base64decode(var.k8s_config.client_key)
    cluster_ca_certificate = base64decode(var.k8s_config.cluster_ca_certificate)
  }
}

locals {
  autoscaling = {
    enabled = true
    maxReplicas = 3
    targetCPUUtilizationPercentage = 80
  }
  affinity = {
    nodeAffinity = {
      requiredDuringSchedulingIgnoredDuringExecution = {
        nodeSelectorTerms = [{
          matchExpressions = [{
            key = "agentpool"
            operator = "In"
            values = [
              "userpool"
            ]
          }]
        }]
      }
    }
  }
  deploymentAnnotations = {
      "wave.pusher.com/update-on-config-change" = "true"
  }
  host = "theidserver.com"
  tls_issuer_name = "letsencrypt"
  tls_issuer_kind = "ClusterIssuer"
  image = {
    repository = "aguacongas/theidserver.duende"
    pullPolicy = "IfNotPresent"
    tag = "next"
  }
  env_settings = var.env_settings
  override_settings = {
    affinity = local.affinity
    seq = {
      affinity = local.affinity
    }
    mysql = {
      primary = {
        affinity = local.affinity
        existingConfigmap = "mysql-master-config"
      }
      secondary = {
        affinity = local.affinity
        existingConfigmap = "mysql-secondary-config"
      }
    }
    redis = {
      master = {
        affinity = local.affinity
      }
      replica = {
        affinity = local.affinity
      }    
    }
    deploymentAnnotations = local.deploymentAnnotations
    appSettings = {
      file = {
        InitialData = {
          Users = [{
            UserName = "olivier.lefebvre@live.com"
          }]
        }
        Serilog = {
          MinimumLevel = {
            ControlledBy = "$controlSwitch"
            Override = {
              "Microsoft.EntityFrameworkCore" = "Warning"
              System = "Warning"
            }
          }
        }
        OpenTelemetryOptions = {
          Trace = {
            ConsoleEnabled = false
            Honeycomb = var.Honeycomb
          }
        }
      }
    }
  }

  wait = false
}

module "theidserver" {
  source = "./modules/terraform-helm-theidserver"
  chart = "C:\\Projects\\Perso\\helm\\charts\\theidserver"
  host = local.host
  tls_issuer_name = local.tls_issuer_name
  tls_issuer_kind = local.tls_issuer_kind
  image = local.image
  env_settings = local.env_settings
  override_settings = local.override_settings
  replica_count = 3

  wait = local.wait
}

resource "helm_release" "cert_manager" {
  name       = "cert-manager"
  repository = "https://charts.jetstack.io"
  chart      = "cert-manager"
  version    = "1.7.2"
  namespace  = "ingress-nginx"
  create_namespace = true
  
  wait = local.wait
}

resource "helm_release" "nginx_ingress" {
  name       = "nginx-ingress"
  repository = "https://helm.nginx.com/stable"
  chart      = "ingress-nginx"
  version    = "4.0.18"
  namespace  = "ingress-nginx"
  create_namespace = true
  
  wait = local.wait
}

resource "helm_release" "wave" {
  name       = "wave"
  repository = "https://wave-k8s.github.io/wave/"
  chart      = "wave"
  version    = "2.0.0"
  namespace  = "wave"
  create_namespace = true
  
  wait = local.wait
}

