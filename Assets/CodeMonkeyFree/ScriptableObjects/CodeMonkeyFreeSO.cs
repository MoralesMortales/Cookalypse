using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace CodeMonkey.FreeWindow {

    [CreateAssetMenu()]
    public class CodeMonkeyFreeSO : ScriptableObject {


        private const long SECONDS_BETWEEN_CONTACTING_WEBSITE = 3600;


        private static CodeMonkeyFreeSO codeMonkeyFreeSO;


        public static CodeMonkeyFreeSO GetCodeMonkeyFreeSO() {
            if (codeMonkeyFreeSO != null) {
                return codeMonkeyFreeSO;
            }
            string[] codeMonkeyFreeSOGuidArray = AssetDatabase.FindAssets(nameof(CodeMonkeyFreeSO));

            foreach (string codeMonkeyFreeSOGuid in codeMonkeyFreeSOGuidArray) {
                string codeMonkeyFreeSOPath = AssetDatabase.GUIDToAssetPath(codeMonkeyFreeSOGuid);
                codeMonkeyFreeSO = AssetDatabase.LoadAssetAtPath<CodeMonkeyFreeSO>(codeMonkeyFreeSOPath);
                return codeMonkeyFreeSO;
            }

            Debug.LogError("Cannot find CodeMonkeyFreeSO!");
            return null;
        }



        public string currentVersion;
        public string subtype;
        public long lastShownTimestamp;

        [SerializeField] private LastUpdateResponse lastUpdateResponse;
        [SerializeField] private long checkedLastUpdateTimestamp;
        [SerializeField] private LastQOTDResponse lastQOTDResponse;
        [SerializeField] private long lastQotdTimestamp;
        [SerializeField] private LastDynamicHeaderResponse lastDynamicHeaderResponse;
        [SerializeField] private long lastDynamicHeaderTimestamp;
        [SerializeField] private WebsiteLatestMessage websiteLatestMessage;
        [SerializeField] private long websiteLatestMessageTimestamp;
        [SerializeField] private LatestVideos websiteLatestVideos;
        [SerializeField] private long websiteLatestVideosTimestamp;




        [Serializable]
        private struct GenericActionJSONData {
            public string at;
            public string st;
        }

        [Serializable]
        private struct WebsiteResponse {
            public int returnCode;
            public string returnText;
        }

        [Serializable]
        private struct WebsiteResponse<T> {
            public int returnCode;
            public T returnText;
        }

        [Serializable]
        public struct LastUpdateResponse {
            public string version;
            public string versionUrl;
        }

        public static void CheckForUpdates(Action<LastUpdateResponse> onFoundUpdate) {
            CodeMonkeyFreeSO codeMonkeyInteractiveSO = GetCodeMonkeyFreeSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (codeMonkeyInteractiveSO.lastUpdateResponse.version == null || codeMonkeyInteractiveSO.lastUpdateResponse.version == "") {
                codeMonkeyInteractiveSO.lastUpdateResponse.version = codeMonkeyInteractiveSO.currentVersion;
            }

            long secondsBetweenCheckingForUpdates = 3600;
            if (unixTimestamp - codeMonkeyInteractiveSO.checkedLastUpdateTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onFoundUpdate(codeMonkeyInteractiveSO.lastUpdateResponse);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.checkedLastUpdateTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "editorwindowversion",
                st = codeMonkeyInteractiveSO.subtype,
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onFoundUpdate(codeMonkeyInteractiveSO.lastUpdateResponse);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            LastUpdateResponse lastUpdateResponse = JsonUtility.FromJson<LastUpdateResponse>(websiteResponse.returnText);
                            codeMonkeyInteractiveSO.lastUpdateResponse = lastUpdateResponse;
                            onFoundUpdate(codeMonkeyInteractiveSO.lastUpdateResponse);
                        } else {
                            // Something went wrong
                            onFoundUpdate(codeMonkeyInteractiveSO.lastUpdateResponse);
                        }
                    }
                } catch (Exception) {
                    onFoundUpdate(codeMonkeyInteractiveSO.lastUpdateResponse);
                }
                unityWebRequest.Dispose();
            };
        }

        [Serializable]
        public struct LastQOTDResponse {
            public string questionId;
            public string questionText;
            public string answerA;
            public string answerB;
            public string answerC;
            public string answerD;
            public string answerE;
        }

        public static void GetLastQOTD(Action<LastQOTDResponse> onResponse) {
            CodeMonkeyFreeSO codeMonkeyInteractiveSO = GetCodeMonkeyFreeSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long secondsBetweenCheckingForUpdates = SECONDS_BETWEEN_CONTACTING_WEBSITE;
            if (unixTimestamp - codeMonkeyInteractiveSO.lastQotdTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.lastQotdTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "getLastQotd",
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            LastQOTDResponse lastQOTDResponse = JsonUtility.FromJson<LastQOTDResponse>(websiteResponse.returnText);
                            codeMonkeyInteractiveSO.lastQOTDResponse = lastQOTDResponse;
                            onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                        } else {
                            // Something went wrong
                            onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                        }
                    }
                } catch (Exception) {
                    onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                }
                unityWebRequest.Dispose();
            };
        }

        [Serializable]
        public struct LastDynamicHeaderResponse {
            public string topImageUrl;
            public string topText;
            public string topLink;
            public string bottomText;
            public string bottomLink;
        }

        public static void GetLastDynamicHeader(Action<LastDynamicHeaderResponse> onResponse) {
            CodeMonkeyFreeSO codeMonkeyInteractiveSO = GetCodeMonkeyFreeSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long secondsBetweenCheckingForUpdates = SECONDS_BETWEEN_CONTACTING_WEBSITE;
            if (unixTimestamp - codeMonkeyInteractiveSO.lastDynamicHeaderTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onResponse(codeMonkeyInteractiveSO.lastDynamicHeaderResponse);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.lastDynamicHeaderTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "getDynamicEmailHeaderJson",
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onResponse(codeMonkeyInteractiveSO.lastDynamicHeaderResponse);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            LastDynamicHeaderResponse lastDynamicHeaderResponse = 
                                JsonUtility.FromJson<LastDynamicHeaderResponse>(websiteResponse.returnText);
                            codeMonkeyInteractiveSO.lastDynamicHeaderResponse = lastDynamicHeaderResponse;
                            onResponse(codeMonkeyInteractiveSO.lastDynamicHeaderResponse);
                        } else {
                            // Something went wrong
                            onResponse(codeMonkeyInteractiveSO.lastDynamicHeaderResponse);
                        }
                    }
                } catch (Exception) {
                    onResponse(codeMonkeyInteractiveSO.lastDynamicHeaderResponse);
                }
                unityWebRequest.Dispose();
            };
        }



        [Serializable]
        public struct WebsiteLatestMessage {
            public string text;
        }

        public static void GetLatestMessage(Action<WebsiteLatestMessage> onResponse) {
            CodeMonkeyFreeSO codeMonkeyInteractiveSO = GetCodeMonkeyFreeSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long secondsBetweenCheckingForUpdates = SECONDS_BETWEEN_CONTACTING_WEBSITE;
            if (unixTimestamp - codeMonkeyInteractiveSO.websiteLatestMessageTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.websiteLatestMessageTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "editorwindowlatestMessage",
                st = codeMonkeyInteractiveSO.subtype,
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            WebsiteLatestMessage websiteLatestMessage = JsonUtility.FromJson<WebsiteLatestMessage>(websiteResponse.returnText);
                            codeMonkeyInteractiveSO.websiteLatestMessage = websiteLatestMessage;
                            onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                        } else {
                            // Something went wrong
                            onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                        }
                    }
                } catch (Exception) {
                    onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                }
                unityWebRequest.Dispose();
            };
        }





        [Serializable]
        public class LatestVideos {
            public LatestVideoSingle[] videos;
        }

        [Serializable]
        public class LatestVideoSingle {
            public string youTubeId;
            public string title;
        }

        public static void GetWebsiteLatestVideos(Action<LatestVideos> onResponse) {
            CodeMonkeyFreeSO codeMonkeyInteractiveSO = GetCodeMonkeyFreeSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long secondsBetweenCheckingForUpdates = SECONDS_BETWEEN_CONTACTING_WEBSITE;
            if (unixTimestamp - codeMonkeyInteractiveSO.websiteLatestVideosTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.websiteLatestVideosTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "getLastVideos",
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            LatestVideos websiteLatestVideos = JsonUtility.FromJson<LatestVideos>(websiteResponse.returnText);
                            codeMonkeyInteractiveSO.websiteLatestVideos = websiteLatestVideos;
                            onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                        } else {
                            // Something went wrong
                            onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                        }
                    }
                } catch (Exception) {
                    onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                }
                unityWebRequest.Dispose();
            };
        }


    }

}