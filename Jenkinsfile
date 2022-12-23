pipeline{
    agent{
        label "node-windows"
    }
    options{
        timestamps()
        skipDefaultCheckout true
        skipStagesAfterUnstable()
        disableConcurrentBuilds()
        buildDiscarder logRotator(artifactDaysToKeepStr: '', artifactNumToKeepStr: '40', daysToKeepStr: '', numToKeepStr: '40')
        copyArtifactPermission '*'
    }
    triggers{
        bitbucketPush overrideUrl: ''
        bitBucketTrigger([
            [
                $class: 'BitBucketPPRPullRequestTriggerFilter', 
                actionFilter: [
                    $class: 'BitBucketPPRPullRequestCreatedActionFilter'
                ]
            ], 
            [
                $class: 'BitBucketPPRRepositoryTriggerFilter', 
                actionFilter: [
                    $class: 'BitBucketPPRRepositoryPushActionFilter', 
                    allowedBranches: 'master, develop, version_64bit, feature/jenkins', 
                    triggerAlsoIfNothingChanged: false,
                    triggerAlsoIfTagPush: false
                ]
            ]
        ])
    }
    environment{
        PROJECT = "nftgame"
        UNITY_VERSION = "2020.3.2f1"
        UNITY_HOME = "C:/Program Files/Unity/Hub/Editor/${UNITY_VERSION}/Editor"
        UNITY_EXE = "${UNITY_HOME}/Unity.exe"
        BASH_EXE = "C:/Program Files/Git/bin/bash.exe"
        BRANCH = "${env.BRANCH_NAME}".replace("/","_")
        TAG_BUILD = "${BRANCH}-${env.BUILD_NUMBER}"
        BUILD_NUMBER = "${env.BUILD_NUMBER}"
        REGISTRY = "104943189603.dkr.ecr.ap-southeast-1.amazonaws.com/${PROJECT}"
        REGISTRY_CREDENTIAL = "ecr:ap-southeast-1:payrollbird-registry-credential"
        CHAT_ID = credentials('nftgame-chat-id')
        BOT_TOKEN = credentials('telegram-token')
    }
    stages{
        stage("Checkout"){
            steps{
                checkout scm
            }
        }
        stage("Build"){
            steps{
                script{
                    WEBGL_FILENAME = "nftgame_${BRANCH}_${BUILD_NUMBER}"

                    bat label: 'Build WebGL', script: "\"${UNITY_EXE}\" -quit -batchMode -nographics -logFile - -projectPath ${WORKSPACE} -executeMethod BuildScript.WebGL"
                    bat label: 'Archive Build WebGL', script: "\"${BASH_EXE}\" -c \"tar -czvf ${WEBGL_FILENAME}.tar.gz ${WEBGL_FILENAME}/\""
                }
            }
            post{
                always{
                    archiveArtifacts allowEmptyArchive: true, artifacts: "${WEBGL_FILENAME}.tar.gz"
                }
            }
        }
        stage("Create Docker Image"){
            agent{
                label "rgb-ec2-fleet"
            }
            steps{
                checkout scm
                copyArtifacts filter: "${WEBGL_FILENAME}.tar.gz", fingerprintArtifacts: true, projectName: "${env.JOB_NAME}", selector: specific("${env.BUILD_NUMBER}")
                sh label: 'unzip artifact', script: "tar -zxvf ${WEBGL_FILENAME}.tar.gz"

                script{
                    dockerImage = docker.build("${PROJECT}:${TAG_BUILD}", "--build-arg WEBFOLDER=${WEBGL_FILENAME} .")
                }
            }
        }
        stage("Test Docker Image") {
            agent{
                label "rgb-ec2-fleet"
            }
            environment{
                PORT = '10000'
            }
            steps{
                script {
                    // Test docker image
                    sh """
                        echo 'Waiting until port ${PORT} not used by other process ...'

                        while nc -z localhost ${PORT}; do
                            sleep 1 && echo -n .
                        done
                    """
                                        
                    sh """
                        docker run -d -p ${PORT}:8080 --name nftgame-${TAG_BUILD} ${PROJECT}:${TAG_BUILD}
                    """

                    sleep 10
                    httpRequest consoleLogResponseBody: true, url: "http://localhost:${PORT}/"
                }
            }
            post{
                always{
                    sh label: 'Remove container', script: "docker stop nftgame-${TAG_BUILD} && docker rm nftgame-${TAG_BUILD}"
                }
            }
        }
        stage("Deploy Development"){
            agent{
                label "rgb-ec2-fleet"
            }
            when{
                anyOf{
                    branch 'master'; branch 'dev'
                }
            }
            steps{
                script {
                    // Push docker image
                    TAG = "dev-latest"
                    docker.withRegistry("https://" + REGISTRY, REGISTRY_CREDENTIAL) {

                        sh label: 'Tag docker image', script: "docker tag ${PROJECT}:${TAG_BUILD} ${REGISTRY}:${TAG_BUILD}"
                        sh label: 'Tag docker image', script: "docker tag ${PROJECT}:${TAG_BUILD} ${REGISTRY}:${TAG}"
                        sh label: 'Push docker image', script: "docker image push ${REGISTRY}:${TAG_BUILD}"
                        sh label: 'Push docker image', script: "docker image push ${REGISTRY}:${TAG}"
                    }

                    withCredentials([sshUserPrivateKey(credentialsId: 'deploy-key', keyFileVariable: 'SSH_KEY', usernameVariable: 'USERNAME')]) {
                    
                        remote = [:]
                        remote.name = 'nftgame-app'
                        remote.host = '172.31.28.11'
                        remote.user = "${USERNAME}"
                        remote.identityFile = "${SSH_KEY}"
                        remote.allowAnyHosts = true

                        sshCommand remote: remote, command: "eval \$(aws ecr get-login --no-include-email --region ap-southeast-1)"
                        sshCommand remote: remote, command: "docker stack deploy --with-registry-auth -c /home/deploy/nftgame/docker-compose.yml nftgame"
                        sshCommand remote: remote, command: "docker rmi -f \$(docker images -f 'dangling=true' -q) || :"
                    }
                }
            }
            post{
                always{
                    sh label: 'Remove dev tag image', script: "docker rmi -f ${REGISTRY}:${TAG_BUILD} || :"
                    sh label: 'Remove dev tag image', script: "docker rmi -f ${REGISTRY}:${TAG} || :"
                }
                success{
                    telegramNotification("Development")
                }
            }
        }
    }
    post{
        failure{
            bat label: 'Telegram Notification', script: "\"${BASH_EXE}\" -c \"C:/telegram -t ${BOT_TOKEN} -c ${CHAT_ID} -M 'Oops.. Something wrong happened when build ***${PROJECT}*** from branch ***${env.BRANCH_NAME}***. Please check [here](${env.BUILD_URL})'\""
        }
    }
}

def telegramNotification(String server) {
    // Git Env
    COMMIT_HASH = sh (script: 'git --no-pager show -s --format=\'%H\'', returnStdout: true).trim()
    COMMIT_HASH_SHORT = sh (script: 'git --no-pager show -s --format=\'%h\'', returnStdout: true).trim()
    COMMITER_NAME = sh (script: 'git --no-pager show -s --format=\'%an\'', returnStdout: true).trim()
    COMMIT_MESSAGE = sh (script: 'git --no-pager show -s --format=\'%B\' -n 1', returnStdout: true).trim()
    COMMIT_MESSAGE = "${COMMIT_MESSAGE}".replace("_","-")
    COMMIT_LINK = "https://bitbucket.org/rollingglory/nft-game/commits/${COMMIT_HASH}"

    MESSAGE = "Changes are deployed to ***${PROJECT} $server*** server by ***${COMMITER_NAME}***.\n[${COMMIT_HASH_SHORT}](${COMMIT_LINK}) : ${COMMIT_MESSAGE}."

    sh label: 'Telegram Notification',  script: "telegram -t ${BOT_TOKEN} -c ${CHAT_ID} -M '${MESSAGE}'"
}