﻿using System.Collections.Generic;
using System.Text;

namespace Vertica.AnalyticsTracker
{
    public class TagTracker
    {
        private string _account;
        private string _dataLayerName;
        private readonly List<MessageBase> _messages;
        private string _environmentAuth;
        private string _environmentPreview;

        public TagTracker()
        {
            _messages = new List<MessageBase>();
        }

        public void SetAccount(string account)
        {
            _account = account;
        }

        public void SetDataLayerName(string dataLayerName)
        {
            _dataLayerName = dataLayerName;
        }

        public void AddMessage(MessageBase message)
        {
            _messages.Add(message);
        }

        public string Render()
        {
            var sb = new StringBuilder();
            RenderDataLayer(sb);

            var environmentQueryParameter = _environmentAuth != null && _environmentPreview != null
                ? string.Format("+'&gtm_auth={0}&gtm_preview={1}'", _environmentAuth, _environmentPreview)
                : "";

            sb.AppendFormat(@"<!-- Google Tag Manager -->
<noscript><iframe src='//www.googletagmanager.com/ns.html?id={0}' height='0' width='0' style='display:none;visibility:hidden'></iframe></noscript>
<script>
(function(w,d,s,l,i){{
w[l]=w[l]||[];
w[l].push({{'gtm.start': new Date().getTime(),event:'gtm.js'}});
var f=d.getElementsByTagName(s)[0], j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';
j.async=true;
j.src='//www.googletagmanager.com/gtm.js?id='+i+dl{2};
f.parentNode.insertBefore(j,f);
}})(window,document,'script','{1}','{0}');
</script>
<!-- End Google Tag Manager -->", _account, _dataLayerName, environmentQueryParameter);

            return sb.ToString();
        }

        public string RenderHeader()
        {
            var sb = new StringBuilder();
            RenderMessages(sb);

            return sb.ToString();
        }

        private void RenderDataLayer(StringBuilder sb)
        {
            sb.AppendLine("<script>");
            sb.AppendFormat("var {0} = [];", _dataLayerName);
            sb.AppendLine();
            RenderMessages(sb);
            sb.AppendLine("</script>");
        }

        private void RenderMessages(StringBuilder sb)
        {
            foreach (var message in _messages)
            {
                sb.AppendLine(message.RenderMessage(_dataLayerName));
            }
        }

        public void SetEnvironmentAuth(string environmentAuth)
        {
            _environmentAuth = environmentAuth;
        }

        public void SetEnvironmentPreview(string environmentPreview)
        {
            _environmentPreview = environmentPreview;
        }
    }
}