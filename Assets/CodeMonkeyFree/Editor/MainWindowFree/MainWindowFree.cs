using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace CodeMonkey.FreeWindow {

    [InitializeOnLoad]
    public class MainWindowFree : EditorWindow {


        [SerializeField] private CodeMonkeyFreeSO codeMonkeyFreeSO;
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private VisualTreeAsset textTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset codeTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset videoTemplateVisualTreeAsset;


        static MainWindowFree() {
            EditorApplication.update += Startup;
        }

        private static void Startup() {
            EditorApplication.update -= Startup;

            try {
                CodeMonkeyFreeSO codeMonkeyInteractiveSO = CodeMonkeyFreeSO.GetCodeMonkeyFreeSO();
                long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long secondsBetweenShowingWindow = 60 * 60 * 24;
                if (unixTimestamp - codeMonkeyInteractiveSO.lastShownTimestamp < secondsBetweenShowingWindow) {
                    // Too soon
                    return;
                }
                
                codeMonkeyInteractiveSO.lastShownTimestamp = unixTimestamp;

                ShowWindow();
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }



        private enum SubWindow {
            MainMenu,
        }


        private VisualElement lectureListVisualElement;
        private VisualElement mainMenuVisualElement;


        [MenuItem("Code Monkey/Code Monkey Free Assets", priority = 0)]
        public static void ShowWindow() {
            try {
                MainWindowFree window = GetWindow<MainWindowFree>();
                window.titleContent = new GUIContent("Code Monkey Free Assets");
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        public static void DestroyChildren(VisualElement containerVisualElement) {
            foreach (VisualElement child in containerVisualElement.Children().ToList()) {
                containerVisualElement.Remove(child);
            }
        }

        public static void AddComplexText(
            VisualTreeAsset textTemplateVisualTreeAsset,
            VisualTreeAsset codeTemplateVisualTreeAsset,
            VisualTreeAsset videoTemplateVisualTreeAsset,
            VisualElement containerVisualElement,
            string text) {
            // Break down complex text and add all components

            // ##REF##video_small, KGFAnwkO0Pk, What are Value Types and Reference Types in C#? (Class vs Struct)##REF##
            // ##REF##code, Console.WriteLine("Qwerty");##REF##

            // Parse HTML
            text = text.Replace("<h1>", "<size=20>");
            text = text.Replace("</h1>", "</size>");
            text = text.Replace("<strong>", "<b>");
            text = text.Replace("</strong>", "</b>");
            text = text.Replace("<p>", "<br>");
            text = text.Replace("</p>", "");

            string refTag = "##REF##";
            string textRemaining = text;
            int safety = 0;
            while (textRemaining.IndexOf(refTag) != -1 && safety < 100) {
                // Found Ref Tag
                int refTagIndex = textRemaining.IndexOf(refTag);

                // Add before text
                string textBefore = textRemaining.Substring(0, refTagIndex);
                AddText(textTemplateVisualTreeAsset, containerVisualElement, textBefore);

                string refData = textRemaining.Substring(refTagIndex + refTag.Length);
                refData = refData.Substring(0, refData.IndexOf(refTag));

                textRemaining = textRemaining.Substring(refTagIndex + refTag.Length);
                textRemaining = textRemaining.Substring(textRemaining.IndexOf(refTag) + refTag.Length);

                string[] refDataArray = refData.Split(',');
                string refType = refDataArray[0].Trim();
                switch (refType) {
                    case "video_small":
                        string youTubeId = refDataArray[1].Trim();
                        string youTubeTitle = refDataArray[2].Trim();
                        string thumbnailUrl = $"https://img.youtube.com/vi/{youTubeId}/mqdefault.jpg";
                        AddVideoReference(videoTemplateVisualTreeAsset, containerVisualElement, thumbnailUrl, youTubeTitle, "https://www.youtube.com/watch?v=" + youTubeId);
                        break;
                    case "code":
                        AddCode(codeTemplateVisualTreeAsset, containerVisualElement, refData.Substring(refType.Length + 1).Trim());
                        break;
                }
                safety++;
            }
            // No more Ref tags found
            AddText(textTemplateVisualTreeAsset, containerVisualElement, textRemaining);
        }

        public static void AddText(VisualTreeAsset textTemplateVisualTreeAsset, VisualElement containerVisualElement, string text) {
            VisualElement textVisualElement = textTemplateVisualTreeAsset.Instantiate();

            Label textLabel = textVisualElement.Q<Label>("textLabel");
            textLabel.text = text;

            containerVisualElement.Add(textVisualElement);
        }

        public static void AddCode(VisualTreeAsset codeTemplateVisualTreeAsset, VisualElement containerVisualElement, string codeString) {
            VisualElement codeVisualElement = codeTemplateVisualTreeAsset.Instantiate();

            Label textLabel = codeVisualElement.Q<Label>("codeLabel");
            textLabel.text = codeString;

            containerVisualElement.Add(codeVisualElement);
        }

        public static void AddVideoReference(VisualTreeAsset videoTemplateVisualTreeAsset, VisualElement containerVisualElement, string imageUrl, string title, string url, VideoReferenceSettings videoReferenceSettings = null) {
            Sprite waitingSprite = null;
            VisualElement videoVisualElement = AddVideoReference(videoTemplateVisualTreeAsset, containerVisualElement, waitingSprite, title, url, videoReferenceSettings);

            UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(imageUrl);
            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                    } else {
                        DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                        VisualElement imageVisualElement = videoVisualElement.Q<VisualElement>("image");
                        imageVisualElement.style.backgroundImage = new StyleBackground(downloadHandlerTexture.texture);
                    }
                } catch (Exception) {
                }
                unityWebRequest.Dispose();
            };
        }

        private static void SetBackgroundImage(VisualElement visualElement, string imageUrl) {
            UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(imageUrl);
            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //Debug.Log("Error Contacting URL: " + unityWebRequest.error);
                        //DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                        //Debug.Log(downloadHandlerTexture.error);
                        //onError(unityWebRequest.error);
                    } else {
                        DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                        visualElement.style.backgroundImage = new StyleBackground(downloadHandlerTexture.texture);
                    }
                } catch (Exception) {
                }
                unityWebRequest.Dispose();
            };
        }

        public static VisualElement AddVideoReference(VisualTreeAsset videoTemplateVisualTreeAsset, VisualElement containerVisualElement, Sprite sprite, string title, string url, VideoReferenceSettings videoReferenceSettings = null) {
            VisualElement videoVisualElement = videoTemplateVisualTreeAsset.Instantiate();

            VisualElement videoContainer = videoVisualElement.Q<VisualElement>("videoContainer");
            videoContainer.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                Debug.Log("Clicked: " + url);
                Application.OpenURL(url);
            });

            VisualElement imageVisualElement = videoContainer.Q<VisualElement>("image");
            imageVisualElement.style.backgroundImage = new StyleBackground(sprite);

            Label textLabel = videoContainer.Q<Label>("titleLabel");
            textLabel.text = title;

            if (videoReferenceSettings != null) {
                if (videoReferenceSettings.height != null) {
                    imageVisualElement.style.height = new StyleLength(videoReferenceSettings.height.Value);
                }
                if (videoReferenceSettings.fontSize != null) {
                    textLabel.style.fontSize = new StyleLength(videoReferenceSettings.fontSize.Value);
                }
            }

            containerVisualElement.Add(videoVisualElement);

            return videoVisualElement;
        }

        public class VideoReferenceSettings {
            public float? height;
            public float? fontSize;
        }

        private SubWindow GetActiveSubWindow() {
            /*if (lectureListVisualElement.style.display == DisplayStyle.Flex) {
                return SubWindow.LectureList;
            }*/
            return SubWindow.MainMenu;
        }

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement rootVisualTreeAsset = visualTreeAsset.Instantiate();
            rootVisualTreeAsset.style.flexGrow = 1f;
            root.Add(rootVisualTreeAsset);

            lectureListVisualElement = root.Q<VisualElement>("lectureList");
            mainMenuVisualElement = root.Q<VisualElement>("mainMenu");

            root.Q<Label>("versionLabel").text = CodeMonkeyFreeSO.GetCodeMonkeyFreeSO().currentVersion;

            Button lectureListButton = mainMenuVisualElement.Q<Button>("lectureListButton");
            lectureListButton.RegisterCallback((ClickEvent clickEvent) => {
                //ShowLectureButtons();
            });

            ShowMainMenu();
        }

        private void ShowMainMenu() {
            lectureListVisualElement.style.display = DisplayStyle.None;
            mainMenuVisualElement.style.display = DisplayStyle.Flex;

            // Check for updates
            CodeMonkeyFreeSO.CheckForUpdates((CodeMonkeyFreeSO.LastUpdateResponse lastUpdateResponse) => {
                if (codeMonkeyFreeSO.currentVersion == lastUpdateResponse.version) {
                    mainMenuVisualElement.Q<VisualElement>("checkingForUpdates").style.display = DisplayStyle.None;
                    return;
                }

                VisualElement checkingForUpdatesVisualElement =
                    mainMenuVisualElement.Q<VisualElement>("checkingForUpdates");
                checkingForUpdatesVisualElement.style.display = DisplayStyle.Flex;
                Label textLabel = checkingForUpdatesVisualElement.Q<Label>();
                textLabel.text = "New version available!\n" +
                    codeMonkeyFreeSO.currentVersion + " -> " + lastUpdateResponse.version + "\n" +
                    "<u>Click here!</u>";

                textLabel.RegisterCallback((ClickEvent clickEvent) => {
                    Application.OpenURL(lastUpdateResponse.versionUrl);
                });
            });

            // Message
            VisualElement messageVisualElement =
                mainMenuVisualElement.Q<VisualElement>("message");

            CodeMonkeyFreeSO.GetLatestMessage((CodeMonkeyFreeSO.WebsiteLatestMessage websiteLatestMessage) => {
                messageVisualElement.Q<Label>("messageLabel").text = websiteLatestMessage.text;
            });


            // QOTD
            VisualElement qotdVisualElement =
                mainMenuVisualElement.Q<VisualElement>("qotd");

            Action openQotdURL = () => {
                string qotdUrl = "https://unitycodemonkey.com/qotd_ask.php?q=30";
                Application.OpenURL(qotdUrl);
            };
            qotdVisualElement.RegisterCallback((ClickEvent clickEvent) => {
                openQotdURL();
            });

            qotdVisualElement.Q<Label>("questionLabel").text = "...";
            qotdVisualElement.Q<Button>("answerAButton").style.display = DisplayStyle.None;
            qotdVisualElement.Q<Button>("answerBButton").style.display = DisplayStyle.None;
            qotdVisualElement.Q<Button>("answerCButton").style.display = DisplayStyle.None;
            qotdVisualElement.Q<Button>("answerDButton").style.display = DisplayStyle.None;
            qotdVisualElement.Q<Button>("answerEButton").style.display = DisplayStyle.None;

            CodeMonkeyFreeSO.GetLastQOTD((CodeMonkeyFreeSO.LastQOTDResponse lastQOTDResponse) => {
                openQotdURL = () => {
                    string qotdUrl = "https://unitycodemonkey.com/qotd_ask.php?q=" + lastQOTDResponse.questionId;
                    Application.OpenURL(qotdUrl);
                };

                qotdVisualElement.Q<Label>("questionLabel").text = lastQOTDResponse.questionText;
                if (!string.IsNullOrEmpty(lastQOTDResponse.answerA)) {
                    qotdVisualElement.Q<Button>("answerAButton").style.display = DisplayStyle.Flex;
                    qotdVisualElement.Q<Button>("answerAButton").text = lastQOTDResponse.answerA;
                }
                if (!string.IsNullOrEmpty(lastQOTDResponse.answerB)) {
                    qotdVisualElement.Q<Button>("answerBButton").style.display = DisplayStyle.Flex;
                    qotdVisualElement.Q<Button>("answerBButton").text = lastQOTDResponse.answerB;
                }
                if (!string.IsNullOrEmpty(lastQOTDResponse.answerC)) {
                    qotdVisualElement.Q<Button>("answerCButton").style.display = DisplayStyle.Flex;
                    qotdVisualElement.Q<Button>("answerCButton").text = lastQOTDResponse.answerC;
                }
                if (!string.IsNullOrEmpty(lastQOTDResponse.answerD)) {
                    qotdVisualElement.Q<Button>("answerDButton").style.display = DisplayStyle.Flex;
                    qotdVisualElement.Q<Button>("answerDButton").text = lastQOTDResponse.answerD;
                }
                if (!string.IsNullOrEmpty(lastQOTDResponse.answerE)) {
                    qotdVisualElement.Q<Button>("answerEButton").style.display = DisplayStyle.Flex;
                    qotdVisualElement.Q<Button>("answerEButton").text = lastQOTDResponse.answerE;
                }
            });


            // Dynamic Message
            VisualElement dynamicMessageVisualElement =
                mainMenuVisualElement.Q<VisualElement>("dynamicMessage");

            Func<string> getDynamicMessageURL = () => "https://unitycodemonkey.com/";
            dynamicMessageVisualElement.RegisterCallback((ClickEvent clickEvent) => {
                Application.OpenURL(getDynamicMessageURL());
            });


            // Latest Videos
            VisualElement latestVideosVisualElement =
                mainMenuVisualElement.Q<VisualElement>("latestVideos");

            latestVideosVisualElement.Q<VisualElement>("_1Container").Clear();
            latestVideosVisualElement.Q<VisualElement>("_2Container").Clear();
            latestVideosVisualElement.Q<VisualElement>("_3Container").Clear();
            latestVideosVisualElement.Q<VisualElement>("_4Container").Clear();



            CodeMonkeyFreeSO.GetWebsiteLatestVideos((CodeMonkeyFreeSO.LatestVideos latestVideos) => {
                AddLatestVideoReference(latestVideos.videos[0], latestVideosVisualElement.Q<VisualElement>("_1Container"));
                AddLatestVideoReference(latestVideos.videos[1], latestVideosVisualElement.Q<VisualElement>("_2Container"));
                AddLatestVideoReference(latestVideos.videos[2], latestVideosVisualElement.Q<VisualElement>("_3Container"));
                AddLatestVideoReference(latestVideos.videos[3], latestVideosVisualElement.Q<VisualElement>("_4Container"));
            });

            void AddLatestVideoReference(CodeMonkeyFreeSO.LatestVideoSingle latestVideoSingle, VisualElement containerVisualElement) {
                string thumbnailUrl = $"https://img.youtube.com/vi/{latestVideoSingle.youTubeId}/mqdefault.jpg";
                string url = $"https://unitycodemonkey.com/video.php?v={latestVideoSingle.youTubeId}";
                AddVideoReference(
                    videoTemplateVisualTreeAsset,
                    containerVisualElement,
                    thumbnailUrl,
                    latestVideoSingle.title,
                    url,
                    new VideoReferenceSettings {
                        height = 80,
                        fontSize = 9,
                    }
                );
            }


            // DynamicHeader
            VisualElement dynamicHeaderVisualElement =
                mainMenuVisualElement.Q<VisualElement>("dynamicHeader");

            dynamicHeaderVisualElement.Q<Label>("text").text = "...";
            dynamicHeaderVisualElement.Q<VisualElement>("image").style.display = DisplayStyle.None;

            Func<string> getTopLinkUrl = () => "https://unitycodemonkey.com/";
            dynamicHeaderVisualElement.Q<VisualElement>("image").RegisterCallback((ClickEvent clickEvent) => {
                Application.OpenURL(getTopLinkUrl());
            });

            CodeMonkeyFreeSO.GetLastDynamicHeader((CodeMonkeyFreeSO.LastDynamicHeaderResponse lastDynamicHeaderResponse) => {
                getTopLinkUrl = () => "https://cmonkey.co/" + lastDynamicHeaderResponse.topLink;

                dynamicHeaderVisualElement.Q<VisualElement>("image").style.display = DisplayStyle.Flex;
                if (lastDynamicHeaderResponse.topImageUrl.Substring(lastDynamicHeaderResponse.topImageUrl.Length - 4) != ".gif") {
                    // Not a gif
                    SetBackgroundImage(dynamicHeaderVisualElement.Q<VisualElement>("image"), lastDynamicHeaderResponse.topImageUrl);
                }

                dynamicHeaderVisualElement.Q<Label>("text").text =
                    "<a href='https://cmonkey.co/" + lastDynamicHeaderResponse.topLink + "'><u>" + lastDynamicHeaderResponse.topText + "</u></a>\n\n" +
                    "<a href='https://cmonkey.co/" + lastDynamicHeaderResponse.bottomLink + "'><u>" + lastDynamicHeaderResponse.bottomText + "</u></a>";
            });
        }


    }





}