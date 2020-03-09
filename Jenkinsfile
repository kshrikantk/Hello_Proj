#!/usr/bin/env groovy
@Library('jenkins_mylib@master')  //Imported Jenkins library from branch master 
import org.common.*
import hudson.model.*
import groovy.transform.Field

//Variables declaration 
@Field String unstableCauses = ""
@Field String change = ""
@Field errorCounter = 0
@Field unstableCounter = 0
@Field mailRecipients = ""

//Project specific variable to make it more reusable

branchName = BRANCH_NAME    // it gives branch name of the branch as it is a multibranch pipeline. 
COMPONENT = "sample"
repo = scm.userRemoteConfigs[0].url 
mailList = 'shrikant.khawale@3ds.com'
newbuildName = "Sample"
slnFile = 'SampleProject.sln'
projFile = "SampleProject.csproj"
conf = "Release"
platform = "x86"

node("master") {
  common = new SSD.Jenkins.Common()   //defining object for library 
  
  // Cleaning workspace before starting 
  stage('Cleanup') {   
   step([$class: 'WsCleanup', cleanWhenFailure: true])
  }

  //Checkout using common.check method
  stage('Checkout') {
    bat """ git config --system core.longpaths true """
    common.check(repo, branchName)

    echo "Gathering SCM changes"
    //getting last commit msg to the variable to display in email. 
    change += common.getChangeString()
    echo "$change"

  }
  
  if (errorCounter != 0) {
    //in case of error to send email , providing required arguments
    common.emailnotify(branchName, unstableCounter, errorCounter, change, mailList, unstableCauses)
  }

  stage('Versioning') {
    try {
      // generating version using 'common.getVersion' method. 
      (full_version, prod_version, full_version_sha, nuget_vrsion) = common.getVersion(branchName)
    }
    catch (e) {
      errorCounter++
      println "An error occured: " + e
      unstableCauses = "${e}"
    }
  }

  stage('Naming') {
    // renaming existing build name with new 
    buildName "${newbuildName}_${branchName}_$prod_version"
  }

  stage('Nuget restore') {
    // restoring nuget 
    bat """ nuget restore ${slnFile} -Verbosity detailed -PackagesDirectory packages """
  }

  if (errorCounter != 0) {
     //in case of error to send email , providing required arguments
    common.emailnotify(branchName, unstableCounter, errorCounter, change, mailList, unstableCauses)
  }

  stage('Build') {
    try {
      // Running Msbuild 
      def msBuild = tool name: 'MSBuild 15', type: 'hudson.plugins.msbuild.MsBuildInstallation'
      println msBuild
      bat """"${msBuild}\\msbuild.exe" ${slnFile} /verbosity:m /p:AsmVersion=${full_version} /p:ProdVersion=${prod_version}" """
    }
    catch (e) {
      errorCounter++
      println "An error occured: " + e
      unstableCauses = "${e}"
    }
  }

  if (errorCounter != 0) {
     //in case of error to send email , providing required arguments
    common.emailnotify(branchName, unstableCounter, errorCounter, change, mailList, unstableCauses)
  }

  stage('Unit tests') {
    try {
      //running xUnit test using common.runXunit Method 
      (unstableCauses, unstableCounter, unitTestResult) = common.runXUnitTest()
    }
    catch (e) {
      errorCounter++
      println "An error occured: " + e
      unstableCauses = "${e}"
    }

    step([$class: 'XUnitBuilder', thresholds: [[$class: 'FailedThreshold', unstableThreshold: '1']], tools: [[$class: 'XUnitDotNetTestType', pattern: '*results.xml']]])
    buildDescription '<h3>Quick links</h3> Document :<a href="http://eu-muc-iss01:8081/POS/html/">Doxygen Report</a> <p> Unit Tests: $unitTestResult </p>'
  }

  stage('Code Coverage') {
    // To get codecoverage report 
    common.runCodeCoverageXunit()
  }

  stage('HTML Report Publish') {
    //To publish HTML report of Code coverage 
    def reportDir = "${WORKSPACE}\\UnitTests\\report"
    def reportFiles = 'index.html'
    def reportName = 'CodeCoverageReport'
    def reportTitles = 'Sample Code Coverage Report'

    common.runHtmlPublish(reportDir, reportFiles, reportName, reportTitles)
  }

  stage('Packaging Nuget') {
    // To pack nuget 
  	common.runNugetPack(projFile, conf, platform)
  }

  if (branchName == 'master' || branchName == 'develop' || (branchName).startsWith('release')) {
    stage('Nuget Push') {
      // To push nugets to Artifactory 
      common.runNugetPush()
    }
  }

  if (branchName == 'master' || branchName == 'develop' || (branchName).startsWith('release')) {
    if (errorCounter > 0) {
      // Incase of failure master release and develop branch should create JIRA ticket 
      stage('JIRA TICKET') {
        def JIRA_SITE = 'JIRA'
        def key = 'DI'
        def Components = $COMPONENT

        common.createJIRATicket(JIRA_SITE, key, Components)
      }
    }
  }

  stage('Archive Artifacts') {
    def zip = tool name: '7zip', type: 'com.cloudbees.jenkins.plugins.customtools.CustomTool'
    bat """
    if not exist "build_output" mkdir "build_output"
    "$zip"\\7z.exe a ${workspace}\\build_output\\Sample_${prod_version}.zip ${workspace}\\Core.Application\\bin\\x86\\Release\\*
      """
    archiveArtifacts 'build_output\\*.zip'
  }

  if (errorCounter != 0) {
     //in case of error to send email , providing required arguments
    common.emailnotify(branchName, unstableCounter, errorCounter, change, mailList, unstableCauses)
  }

}
