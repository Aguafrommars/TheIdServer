# k8s connection settings are stored in k8s_config variable in Terraform cloud
provider "kubernetes" {
  host = var.k8s_config.host
  token = var.k8s_config.token
  client_certificate = base64decode(var.k8s_config.client_certificate)
  client_key = base64decode(var.k8s_config.client_key)
  cluster_ca_certificate = var.k8s_config.cluster_ca_certificate != null ? base64decode(var.k8s_config.cluster_ca_certificate) : ""
  insecure = var.k8s_config.insecure
}

# k8s connection settings are stored in k8s_config variable in Terraform cloud
provider "helm" {
  kubernetes {
    host = var.k8s_config.host
    token = var.k8s_config.token
    client_certificate = base64decode(var.k8s_config.client_certificate)
    client_key = base64decode(var.k8s_config.client_key)
    cluster_ca_certificate = var.k8s_config.cluster_ca_certificate != null ? base64decode(var.k8s_config.cluster_ca_certificate) : ""
    insecure = var.k8s_config.insecure
  }
}

locals {
  # set node affinity to userpool nodes
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
  # enable wave on config change
  deploymentAnnotations = {
    "wave.pusher.com/update-on-config-change" = "true"
  }
  host = "theidserver.com"
  tls_issuer_name = "letsencrypt"
  tls_issuer_kind = "ClusterIssuer"
  image = {
    repository = "aguacongas/theidserver.duende"
    pullPolicy = "Always"
    tag = "next"
  }
  # SendGrid settings are store in env_settings var in Terraform cloud
  env_settings = var.env_settings
  override_settings = {
    # set node affinity to userpool nodes
    affinity = local.affinity
    seq = {
      # set node affinity to userpool nodes
      affinity = local.affinity
    }
    mysql = {
      image = {
        debug = true
      }      
      primary = {
        # set node affinity to userpool nodes
        affinity = local.affinity
        # user custom master config (max_connections=512)
        # existingConfigmap = "mysql-master-config"
      }
      secondary = {
        # set node affinity to userpool nodes
        affinity = local.affinity
        # user custom secondary config (max_connections=512)
        # existingConfigmap = "mysql-secondary-config"
      }
    }
    redis = {
      master = {
        # set node affinity to userpool nodes
        affinity = local.affinity
      }
      replica = {
        # set node affinity to userpool nodes
        affinity = local.affinity
      }    
    }
    # enable wave on config change
    deploymentAnnotations = local.deploymentAnnotations
    appSettings = {
      file = {
        # override serilog settings
        Serilog = {
          MinimumLevel = {
            ControlledBy = "$controlSwitch"
            Override = {
              "Microsoft.EntityFrameworkCore" = "Warning"
              System = "Warning"
            }
          }
        }
        # enable honeycomb
        OpenTelemetryOptions = {
          Trace = {
            ConsoleEnabled = false
            Honeycomb = var.honeycomb
          }
        }
      }
    }
  }

  wait = false
}

# Install ingress-nginx
resource "helm_release" "nginx_ingress" {
  name       = "nginx-ingress"
  repository = "https://kubernetes.github.io/ingress-nginx"
  chart      = "ingress-nginx"
  version    = "4.0.19"
  namespace  = "ingress-nginx"
  create_namespace = true

  # enable ssl passthrough to have end-to-end encryption
  set {
    name = "controller.extraArgs.enable-ssl-passthrough"
    value = true
  }
  
  wait = local.wait
}

# Install cert_manager to manage TLS certificates with letsencrypt
resource "helm_release" "cert_manager" {
  name       = "cert-manager"
  repository = "https://charts.jetstack.io"
  chart      = "cert-manager"
  version    = "1.7.2"
  namespace  = "ingress-nginx"
  create_namespace = true

  # uncomment it on 1st deploy

  # set {
  #  name = "installCRDs"
  #  value = true
  #}
  
  wait = local.wait
}

# Instal wave to restart nodes on config changes
resource "helm_release" "wave" {
  name       = "wave"
  repository = "https://wave-k8s.github.io/wave/"
  chart      = "wave"
  version    = "2.0.0"
  namespace  = "wave"
  create_namespace = true
  
  wait = local.wait
}

# creates ClusterIssuer
resource "kubernetes_manifest" "cluster_issuer" {
  manifest = {
    apiVersion = "cert-manager.io/v1"
    kind = "ClusterIssuer"
    metadata = {
      name = "letsencrypt"
    }
    spec = {
      acme = {
        email = "aguacongas@gamil.com"
        server = "https://acme-v02.api.letsencrypt.org/directory"
        privateKeySecretRef = {
          name = "letsencrypt-secrets"
        }
        solvers = [{
          http01 = {
            ingress = {
              class = "nginx"    
            }    
          }
        }]
      }    
    }
  }  
}

# create ns
resource "kubernetes_namespace" "theidserver_namespace" {
  metadata {
    name = "theidserver"
  }  
}

# store mysql master config (max_connections=512)
resource "kubernetes_config_map" "mysql_master_config" {
  metadata {
    name = "mysql-master-config"
    namespace = "theidserver"
  }

  data = {
    "my.cnf" = file("${path.module}/my-master.cnf")
  }
}

# store mysql secondary config (max_connections=512)
resource "kubernetes_config_map" "mysql_scondary_config" {
  metadata {
    name = "mysql-secondary-config"
    namespace = "theidserver"
  }

  data = {
    "my.cnf" = file("${path.module}/my-secondary.cnf")
  }
}

# install TheIdServer cluster with MySql cluster, Redis cluster and Seq server
module "theidserver" {
  source = "Aguafrommars/theidserver/helm"
  
  host = local.host
  tls_issuer_name = local.tls_issuer_name
  tls_issuer_kind = local.tls_issuer_kind
  image = local.image
  env_settings = local.env_settings
  override_settings = local.override_settings
  replica_count = 3
  create_namespace = true

  wait = local.wait
}


