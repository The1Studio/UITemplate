namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using System.IO;
    using System.Linq;
    using System.Xml;
    using UnityEngine;

    public static class AndroidManifestMigration
    {
        //Reade XML file Plugin/Andoird/AndroidManifest.xml then update the content
        public static void UpdateAndroidManifest()
        {
            var androidManifestPath = Path.Combine(Application.dataPath, "Plugins", "Android", "AndroidManifest.xml");
            var doc                 = new XmlDocument();
            doc.Load(androidManifestPath);
            
            AdServicePropertyMigration(doc);
            FirebaseMessagingMigration(doc);

            //save manifest
            doc.Save(androidManifestPath);
        }
        
        /// <summary>
        /// Add tools:replace="android:exported" to the service node with android:name="com.google.firebase.messaging.MessageForwardingService"
        /// </summary>
        /// <param name="doc"></param>
        private static void FirebaseMessagingMigration(XmlDocument doc)
        {
            var serviceNodeList = doc.GetElementsByTagName("service");
            foreach (XmlNode serviceNode in serviceNodeList)
            {
                //select node with attribute android:name="com.google.firebase.messaging.MessageForwardingService"
                if (serviceNode.Attributes?["android:name"]?.Value != "com.google.firebase.messaging.MessageForwardingService") return;
                //check if the node has tools:replace attribute
                if (serviceNode.Attributes?["tools:replace"] != null) return;
                //check if android:exported==true then continue
                if (serviceNode.Attributes?["android:exported"]?.Value == "true") return;
                var serviceElement = (XmlElement)serviceNode;
                serviceElement.SetAttribute("replace", "http://schemas.android.com/tools", "android:exported");
            }
        }

        /// <summary>
        /// Add <property android:name="android.adservices.AD_SERVICES_CONFIG" android:resource="@xml/ga_ad_services_config" tools:replace="android:resource"/>
        /// </summary>
        /// <param name="doc"></param>
        private static void AdServicePropertyMigration(XmlDocument doc)
        {
            var applicationList = doc.GetElementsByTagName("application");
            foreach (XmlNode application in applicationList)
            {
                // Check if the node already has the property android.adservices.AD_SERVICES_CONFIG
                if (application.ChildNodes.Cast<XmlNode>().Any(node => 
                        node.Name == "property" && 
                        node.Attributes?["android:name"]?.Value == "android.adservices.AD_SERVICES_CONFIG"))
                {
                    // If the property already exists, remove it
                    application.RemoveChild(application.ChildNodes.Cast<XmlNode>().First(node => 
                        node.Name == "property" && 
                        node.Attributes?["android:name"]?.Value == "android.adservices.AD_SERVICES_CONFIG"));
                    // continue;
                }
                
                //Add <property android:name="android.adservices.AD_SERVICES_CONFIG" android:resource="@xml/ga_ad_services_config" tools:replace="android:resource"/>
                // Create the new property element
                // var propertyElement = doc.CreateElement("property");
                // propertyElement.SetAttribute("name", "http://schemas.android.com/apk/res/android", "android.adservices.AD_SERVICES_CONFIG");
                // propertyElement.SetAttribute("resource", "http://schemas.android.com/apk/res/android", "@xml/ga_ad_services_config");
                // propertyElement.SetAttribute("replace", "http://schemas.android.com/tools", "android:resource");
                    
                // Append the new property element to the service node
                // application.AppendChild(propertyElement);
            }
        }
    }
}