set -x 

# #### Cluster setup
# 1. Create IAM user for ECS cluster creation and management w/AWS WCS policy AmazonEC2ContainerRegistryFullAccess 
# 2. Create key pair
# 3. Create cluster using ecsContainerRole (which uses AmazonEC2ContainerServiceforEC2Role policy); VPC is created  
#   - use key pair 
 
# ### App setup
# 1. Create task definition & servive
# 2. Run deploy script with env vars below

# ## If running locally, these parameters are needed, can be added to your env vars:
# ## acct Id for url prefix 
# AWS_DEFAULT_REGION=$AWS_DEFAULT_REGION
# AWS_ACCOUNT_ID=$AWS_ACCOUNT_ID
# CLUSTER_NAME=$CLUSTER_NAME
# SERVICE_BASENAME=$SERVICE_BASENAME
# #   (i.e. my-app; -dev will get appended for the dev environment, making my-app-dev)
# ## for tag 
# COMMIT_SHA1=$TRAVIS_COMMIT
# ## td revision, version from CI
# ECR_NAME=$IMAGE_BASENAME
# TASK_DEF_NAME=$IMAAGE_BASENAME
# DESIRED_COUNT=1
# family=defic-svc-prod
# revision=99001

# more bash-friendly output for jq
JQ="jq --raw-output --exit-status"

# create_service() {
#     make_task_def
#     register_definition    
#     aws ecs create-service --service-name $SERVICE_NAME --task-definition $TASK_DEF_NAME --desired-count $DESIRED_COUNT
# }
# # #####################################################################

configure_aws_cli(){
	aws --version
	aws configure set default.region us-east-1
	aws configure set default.output json
}

deploy_cluster() {

    family="sample-webapp-task-family"

    make_task_def
    register_definition
    if [[ $(aws ecs update-service --cluster $CLUSTER_NAME --service $SERVICE_NAME --task-definition $revision | \
                   $JQ '.service.taskDefinition') != $revision ]]; then
        echo "Error updating service."
        return 1
    fi

    # wait for older revisions to disappear
    # not really necessary, but nice for demos
    for attempt in {1..30}; do
        if stale=$(aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME | \
                       $JQ ".services[0].deployments | .[] | select(.taskDefinition != \"$revision\") | .taskDefinition"); then
            echo "Waiting for stale deployments:"
            echo "$stale"
            sleep 5
        else
            echo "Deployed!"
            return 0
        fi
    done
    echo "Service update took too long."
    return 1
}
make_task_def(){
	task_template='
{
  "family": "defic-svc-prod",
  "containerDefinitions": [
    {
      "memory": 366,
      "portMappings": [
        {
          "hostPort": 0,
          "containerPort": 5002,
          "protocol": "tcp"
        }
      ],
      "essential": true,
      "name": "defic-svc-prod",
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        },
        {
          "name": "ASPNETCORE_URLS",
          "value": "http://*:5002"
        },
        {
          "name": "POSTGRES_PASSWORD",
          "value": "<ENTER>"
        }
      ],
      "links": [
        "postgres-defic-svc-prod"
      ],
      "image": "090999229429.dkr.ecr.us-east-1.amazonaws.com/defic-svc:999.12",
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "defic-svc-logs",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "defic-svc-prod-api"
        }
      },
      "cpu": 0,
      "privileged": false
    },
    {
      "memory": 366,
      "portMappings": [
        {
          "hostPort": 0,
          "containerPort": 5432,
          "protocol": "tcp"
        }
      ],
      "essential": true,
      "mountPoints": [
        {
          "containerPath": "/var/lib/postgresql/data",
          "sourceVolume": "volume-defic-svc-prod",
          "readOnly": false
        }
      ],
      "name": "postgres-defic-svc-prod",
      "environment": [
        {
          "name": "POSTGRES_PASSWORD",
          "value": "<ENTER>"
        }
      ],
      "image": "postgres",
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "defic-svc-logs",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "defic-svc-prod-postgres"
        }
      },
      "cpu": 0,
      "privileged": false
    },
    {
      "memory": 366,
      "essential": false,
      "name": "postgres_backups",
      "environment": [
        {
          "name": "AWS_ACCESS_KEY_ID",
          "value": "<ENTER>"
        },
        {
          "name": "AWS_SECRET_ACCESS_KEY",
          "value": "<ENTER>"
        },
        {
          "name": "POSTGRES_PASSWORD",
          "value": "<ENTER>"
        },
        {
          "name": "PREFIX",
          "value": "postgres-backup"
        },
        {
          "name": "S3_BUCKET_NAME",
          "value": "container-db-backup-job-test"
        }
      ],
      "links": [
        "postgres-defic-svc-prod"
      ],
      "image": "090999229429.dkr.ecr.us-east-1.amazonaws.com/defic-svc-dbbackup",
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "defic-svc-logs",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "defic-svc-prod-dbbackup"
        }
      },
      "cpu": 0,
      "privileged": false    }
  ],
  "placementConstraints": [
    {
      "expression": "attribute:environment == production",
      "type": "memberOf"
    }
  ],
  "volumes": [
    {
      "host": {
        "sourcePath": "postgres-defic-svc-prod"
      },
      "name": "volume-defic-svc-prod"
    }
  ]
}'
	
	task_def=$(printf "$task_template" $AWS_ACCOUNT_ID $ECR_NAME $COMMIT_SHA1)
}

register_definition() {

    #if revision=$(aws ecs register-task-definition --container-definitions "$task_def" --family $family | $JQ '.taskDefinition.taskDefinitionArn'); then
    if revision=$(aws ecs register-task-definition --cli-input-json "$task_def" --family $family | $JQ '.taskDefinition.taskDefinitionArn'); then
        echo "Revision: $revision"
    else
        echo "Failed to register task definition"
        return 1
    fi

}

configure_aws_cli

# create_service # THIS SHOULD ONLY BE RUN ONCE

deploy_cluster