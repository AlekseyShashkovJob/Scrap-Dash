using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Infrastructure.Platform
{
    public class ConnectivityProbe
    {
        private static readonly string[] Endpoints = new[]
        {
            "https://clients3.google.com/generate_204",
            "https://www.gstatic.com/generate_204",
            "https://www.apple.com/library/test/success.html"
        };

        public IEnumerator Evaluate(Action<bool> onResult)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                Debug.LogWarning("[Connectivity] Device reports no network.");

            foreach (var endpoint in Endpoints)
            {
                using var req = UnityWebRequest.Head(endpoint);
                req.timeout = 8;
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[Connectivity] Confirmed via: {endpoint}");
                    onResult?.Invoke(true);
                    yield break;
                }

                Debug.LogWarning($"[Connectivity] Failed {endpoint}: {req.error}");
            }

            Debug.LogError("[Connectivity] All endpoints unreachable.");
            onResult?.Invoke(false);
        }
    }
}