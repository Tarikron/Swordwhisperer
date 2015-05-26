using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class DialogLoadXml : MonoBehaviour {

	private string xmlPath;
	private Hashtable Strings;
	private string lang = "de";
	private Dialog dlg;

	public DialogLoadXml(string xmlPath)
	{
		this.xmlPath = xmlPath;		
	}

	public Dictionary<string,Dialog> parseXml()
	{
		if (xmlPath.Length <= 0)
			return null;

		Dictionary<string,Dialog> dialogs  = new Dictionary<string, Dialog>();
		XmlDocument xml = new XmlDocument();
		xml.Load(xmlPath);

		Strings = new Hashtable();
		XmlElement rootElement = xml.DocumentElement;

		if (rootElement.HasChildNodes)
		{
			foreach (XmlElement element in rootElement.ChildNodes)
			{
				string langAtrib = element.GetAttribute("lang").ToString();
				Debug.Log (langAtrib + " == " + lang);
				if (langAtrib == lang)
				{
					XmlNode node = element.FirstChild;
					XmlNode currentNode = node;
					XmlNode lastDialogNode = node;

					do 
					{
						if (currentNode.Name == "dialog")
						{
							//currentNode = dialog
							dlg = new Dialog();
							dlg.event_trigger = currentNode.Attributes["eventTrigger"].Value;
							if (currentNode.HasChildNodes)
							{
								//go to person loop
								lastDialogNode = currentNode;
								currentNode = currentNode.FirstChild;
							}
						}
						if (currentNode.Name == "person")
						{
							//currentNode = person
							dlg.person.order_id = int.Parse(currentNode.Attributes["order"].Value);
							dlg.person.uid = currentNode.Attributes["uid"].Value;
							dlg.person.text = currentNode["text"].InnerText;
							dlg.person.fade_in = int.Parse(currentNode["fade_in"].InnerText);
							dlg.person.fade_out = int.Parse(currentNode["fade_out"].InnerText);
							dlg.person.click_away = int.Parse(currentNode["click_away"].InnerText);
							dlg.person.soundfile = currentNode["soundfile"].InnerText;
							dlg.persons.Add(dlg.person);

							if (currentNode.NextSibling == null)
							{
								//add dialog if there is no person xml tag
								if (!dialogs.ContainsKey(dlg.event_trigger))
									dialogs.Add(dlg.event_trigger,dlg);

								//set currentnode to last dialog
								currentNode = lastDialogNode;
							}
						}
							
						currentNode = currentNode.NextSibling;
					} while (currentNode != null);
					break;
				}
			} //end foreach

			if (dialogs != null)
				return dialogs;
		}

		return null;
	}


}
