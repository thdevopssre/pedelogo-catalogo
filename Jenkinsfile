pipeline {
    agent any
    
    environment {
        SCANNER_HOME = tool 'sonar-scanner'
    }

    stages {
        stage('clean workspace') {
            steps {
                cleanWs()
            }
        }

        stage('Checkout from Git') {
            steps {
                git branch: 'main', url: 'https://github.com/thdevopssre/pedelogo-catalogo.git'
            }
        }

        stage("Sonarqube Analysis") {
            steps {
                withSonarQubeEnv('sonar-server') {
                    sh "$SCANNER_HOME/bin/sonar-scanner -Dsonar.projectName=api-produto -Dsonar.projectKey=api-produto"
                }
            }
        }

        stage("Quality Gate") {
            steps {
                script {
                    waitForQualityGate abortPipeline: false, credentialsId: 'Sonar-token' 
                }
            }
        }

        stage('OWASP FS SCAN') {
            steps {
                dependencyCheck additionalArguments: '--scan ./ --disableYarnAudit --disableNodeAudit', odcInstallation: 'DP-Check'
                dependencyCheckPublisher pattern: '**/dependency-check-report.xml'
            }
        }
        
        stage('Docker Scout FS') {
            steps {
                script {
                   withDockerRegistry(credentialsId: 'docker', toolName: 'docker'){
                       sh 'docker-scout quickview fs://.'
                       sh 'docker-scout cves fs://.'
                   }
                }
            }
        }

        stage('TRIVY FS SCAN') {
            steps {
                sh "trivy fs . > trivyfs.txt"
            }
        }

        stage("Docker Build & Push") {
            steps {
                script {
                    withDockerRegistry(credentialsId: 'docker', toolName: 'docker') {
                        dockerapp = docker.build("thsre/api-produto:${env.BUILD_ID}", '-f ./src/PedeLogo.Catalogo.Api/Dockerfile .')
                        docker.withRegistry('https://registry.hub.docker.com', 'docker') {
                            dockerapp.push('latest')
                            dockerapp.push("${env.BUILD_ID}")
                        }
                    }
                }
            }
        }

        stage("TRIVY") {
            steps {
                sh "trivy image thsre/api-produto:latest > trivyimage.txt" 
            }
        }

        stage('Deploy to Container') {
            steps {
                sh 'docker run -d -p 8081:80 thsre/api-produto:latest'
            }
        }
        stage('Deploy API helm chart on EKS') {
            steps {
                script {
                    sh ('aws eks update-kubeconfig --name MATRIX-EKS --region us-east-1')
                    sh "kubectl get ns"
                    sh "helm install api-produto ./api-produto"
                }
            }
        }
    }
}
