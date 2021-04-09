using System.Collections.Generic;

using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TreeView1
{
    public class JsonHelper
    {
        public static TreeViewNode JsonToTree(JArray obj, string nodeName)
        {
            if (obj == null)
                return null;

            string itemCountString = obj.Count.ToString() + " item" + (obj.Count > 1 ? "s" : "");
            var parent = new TreeViewNode() { Content = new KeyValuePair<string, string>(nodeName, itemCountString), IsExpanded = true };
            int index = 0;

            foreach (JToken token in obj)
            {
                if (token.Type == JTokenType.Object)
                {
                    parent.Children.Add(JsonToTree((JObject)token, $"{nodeName}[{index++}]"));
                }
                else if (token.Type == JTokenType.Array)
                {
                    parent.Children.Add(JsonToTree((JArray)token, $"{nodeName}[{index++}]"));
                }
                else
                {
                    parent.Children.Add(new TreeViewNode()
                    {
                        Content = new KeyValuePair<string, JToken>($"{nodeName}[{index++}]", token), IsExpanded = true
                    });
                }
            }

            return parent;
        }

        public static TreeViewNode JsonToTree(JObject obj, string nodeName)
        {
            if (obj == null)
                return null;

            var parent = new TreeViewNode() { Content = new KeyValuePair<string, string>(nodeName, obj.Count.ToString() + " items"), IsExpanded = true };

            foreach (KeyValuePair<string, JToken> pair in obj)
            {
                if (pair.Value.Type == JTokenType.Object)
                {
                    parent.Children.Add(JsonToTree((JObject)pair.Value, pair.Key));
                }
                else if (pair.Value.Type == JTokenType.Array)
                {
                    parent.Children.Add(JsonToTree((JArray)pair.Value, pair.Key));
                }
                else
                {
                    // MP! WinUI will not crash if this line is commented out??
                    parent.Children.Add(GetChild(pair));
                }
            }

            return parent;
        }

        private static TreeViewNode GetChild(KeyValuePair<string, JToken> pair)
        {
            if (pair.Value == null)
                return null;

            // MP! WinUI crashes here??
            TreeViewNode child = new TreeViewNode()
            {
                Content = pair, IsExpanded = true
            };

            return child;
        }

        public static void LoadTree(TreeView sender, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;

            sender.RootNodes.Clear();

            JContainer json;
            try
            {
                if (content.StartsWith("["))
                {
                    json = JArray.Parse(content);
                    sender.RootNodes.Add(JsonToTree((JArray)json, "Root"));
                }
                else
                {
                    json = JObject.Parse(content);
                    sender.RootNodes.Add(JsonToTree((JObject)json, "Root"));
                }
            }
            catch (JsonReaderException)
            {
                // invalid json
            }
        }
    }
}
