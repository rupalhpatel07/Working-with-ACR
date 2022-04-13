# Working-with-ACR

## Create new repositary in your GitHub account
* Login to GitHub and navigate to repositaries to create a new one
* Clone the repo to a local directory and open in VS Code

## Create a new web api using VS code in the cloned directory
```
dotnet new webapi --no-https
```

## Add a Dockerfile
```
Ctrl+Shift+P for Windows
CMD+Shift+P for Windows
```
* Search for docker file to add dockerfile to the workspace
* Push the code to your GitHub repo using the Git extention of VS Code

## Tag and push image locally
```
docker login bcglabacr01.azurecr.io
Username: bcglabacr01
Password: vQign8/X5HO3XhfXFWFW05CtHqpYV+dM
docker images
docker tag <source image> bcglabacr01.azurecr.io/<target image>:<tag>
docker push bcglabacr01.azurecr.io/<target image>:<tag>
```

## Navigate to GitGub and add an action
* Click on Actions
* Create new action using the Docker template -> click on configure
* Use the below yaml to create the CI defination for building and pushing the docker image to Azure container registry:
```
name: Docker Image CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:

  build_push_image:

    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    - name: Build the Docker image
      run: docker build . --file Dockerfile --tag bcglabacr01.azurecr.io/workingwithacr:latest
     
    - name: Azure Container Registry Login
      uses: Azure/docker-login@v1
      with:
        # Container registry username
        username: bcglabacr01
        # Container registry password
        password: vQign8/X5HO3XhfXFWFW05CtHqpYV+dM 
        # Container registry server url
        login-server: bcglabacr01.azurecr.io
        
    - name: Push Image  
      run: docker push bcglabacr01.azurecr.io/workingwithacr:latest
   ```
   ** Commit into the readme file of your repo to trigger the action automatically
   
## Azure Login 
```
Az login 
Az account set -s <id>
Az account show
```

## Login to AKS cluster
```
az account set --subscription b214611b-9a79-4e7e-afb0-3d9785737f10
az aks get-credentials --resource-group VMware-RG --name vmwareaks01
```

## Only when you DO NOT have a local image to work with:
* Sample .NET Core Web App image:
```
docker run -it --rm -p 8000:80 --name aspnetcore_sample mcr.microsoft.com/dotnet/samples:aspnetapp
```

## Retag and push to ACR:
```
docker login bcglabacr01.azurecr.io
Username: bcglabacr01
Password: vQign8/X5HO3XhfXFWFW05CtHqpYV+dM
docker images
docker tag <source image> bcglabacr01.azurecr.io/<target image>:<tag>
docker push bcglabacr01.azurecr.io/<target image>:<tag>
```


## K8s Management Techniques
* https://kubernetes.io/docs/concepts/overview/working-with-objects/object-management/

## Imperative Deployment

* Deploy the app:
```
kubectl create deployment <your-name>01 --image=bcglabacr01.azurecr.io/<your-image-name>:v1 --replicas=2

kubectl get deployments

kubectl describe deployment <your-deployment-name>

kubectl expose deployment < your-deployment-name> --type=LoadBalancer --port=80 --target-port=80 #map service port 80 to container port 80

kubectl scale deployment < your-deployment-name> --replicas=3
```

## AKS Pods and Deployments using declarative method
* Add deployment.yaml -- Generate the YAML file using K8s extention
* Add service.yaml -- Generate the YAML file using K8s extention
* Apply the above desired state:
```
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml
```

## Add support for deploying into AKS
* Copy the contents of the Kubeconfig and create a repositary secret in GitHub - KUBECONFIG:
```
az aks get-credentials --resource-group VMware-RG --name vmwareaks01 -f kubeconfig-ss
```

* Add new stage to docker-image.yml:
```
deploy:
    runs-on: ubuntu-latest
    needs: build_push_image # Will wait for the execution of the previous job 

    steps:
    - uses: actions/checkout@v2

    - name: Kubernetes set context
      uses: Azure/k8s-set-context@v1
      with:
        kubeconfig: ${{secrets.KUBECONFIG}} # Grab the auth token from GitHub secrets
      id: login

    #Declarative Deployment-02
    - name: Kubernetes Deployment
      run: kubectl apply -f deployment.yaml
    - name: Service Deployment
      run: kubectl apply -f service.yaml
```