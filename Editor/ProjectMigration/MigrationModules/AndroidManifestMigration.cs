namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using System.IO;
    using System.Xml;
    using UnityEngine;

    public static class AndroidManifestMigration
    {
        //Reade XML file Plugin/Andoird/AndroidManifest.xml then update the content
        public static void UpdateAndroidManifest()
        {
            var androidManifestPath    = Path.Combine(Application.dataPath, "Plugins", "Android", "AndroidManifest.xml");
            var doc                    = new XmlDocument();
            doc.Load(androidManifestPath);
            //add 'tools:replace="android:exported"' to <service> element at AndroidManifest.xml where service has attribute android:name="com.google.firebase.messaging.MessageForwardingService"
            var serviceNodeList = doc.GetElementsByTagName("service");
            foreach (XmlNode serviceNode in serviceNodeList)
            {
                //select node with attribute android:name="com.google.firebase.messaging.MessageForwardingService"
                if (serviceNode.Attributes?["android:name"]?.Value != "com.google.firebase.messaging.MessageForwardingService") continue;
                //check if the node has tools:replace attribute
                if (serviceNode.Attributes?["tools:replace"] != null) continue;
                var serviceElement = (XmlElement)serviceNode;
                serviceElement.SetAttribute("replace", "http://schemas.android.com/tools", "android:exported");
            }
            //save manifest
            doc.Save(androidManifestPath);
        }
    }
}