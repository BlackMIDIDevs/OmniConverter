using System;
using DiscordRPC;

namespace OmniConverter
{
    public class DiscordRPC
    {
        private const string _clientId = "1254865981549969449";
        private readonly DiscordRpcClient _client;
        private bool _cleared = true;

        public DiscordRPC()
        {
            _client = new(_clientId);

            _client.OnPresenceUpdate += (sender, e) =>
            {
                if (e.Presence != null)
                {
                    string? pres = e.Presence.ToString();
                    Debug.PrintToConsole(Debug.LogType.Message, pres ?? string.Empty);
                }
            };
        }

        private void Connect()
        {
            if (Program.Settings.RichPresence)
                _client.Initialize();
        }

        public void SetPresence(string state, string details, MIDIConverter.ConvStatus status = MIDIConverter.ConvStatus.Idle)
        {
            if (Program.Settings.RichPresence)
            {
                if (!IsInitialized())
                    Connect();

                Assets assets = new Assets()
                {
                    LargeImageKey = "oclogo",
                    LargeImageText = state,
                    SmallImageText = details
                };

                switch (status)
                {
                    case MIDIConverter.ConvStatus.Idle:
                    default:
                        assets.SmallImageKey = "sleep";
                        break;
                }

                if (IsInitialized())
                {
                    _client.ClearPresence();
                    _client.SetPresence(new RichPresence()
                    {
                        Details = details,
                        State = state,
                        Assets = assets,
                    });
                }

                _cleared = false;
            }
            else
            {
                if (!_cleared)
                {
                    if (IsInitialized())
                        ClearPresence();

                    _cleared = true;
                }
            }
        }

        public void ClearPresence() => _client.ClearPresence();

        public void Update() => _client.Invoke();

        public bool IsInitialized() => _client.IsInitialized;
    }
}
