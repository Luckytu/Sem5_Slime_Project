using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace InstagramGeneration
{
    public class InstagramUI : MonoBehaviour
    {
        public struct SlimeContainer
        {
            public Label username;
            public Label posts;
            public Label followers;
            public Label following;
            public Label lifetime;
            public Label population;
            public Label alivePopulation;
        }
        
        List<SlimeContainer> slimeLabels;
        private List<VisualElement> slimeContainers;
        
        [SerializeField] private UIDocument uiDocument;
        private VisualElement root;

        private VisualElement queueElement;
        private Queue<Label> profileQueue;
        
        private TextField inputField;
        private bool userNameEntered = false;
        [SerializeField] private UnityEvent<string> onUsernameEnter;

        private void Awake()
        {
            profileQueue = new Queue<Label>();
            
            root = uiDocument.rootVisualElement;
            
            initializeElements();
        }

        private void Start()
        {
            inputField = root.Q("EnterInstagram") as TextField;
            inputField?.RegisterCallback<InputEvent>(checkInputEmpty);

            queueElement = root.Q("Queue");
        }

        private void Update()
        {
            if (userNameEntered && Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log(inputField.value);
                onUsernameEnter.Invoke(inputField.value);

                Label label = new Label(inputField.value);
                profileQueue.Enqueue(label);
                queueElement.Add(label);
                
                inputField.value = string.Empty;
            }
        }

        private void checkInputEmpty(InputEvent e)
        {
            Debug.Log(e.newData);
            userNameEntered = !string.IsNullOrWhiteSpace(e.newData);
        }
        
        private void initializeElements()
        {
            slimeContainers = root.Query("Slime").ToList();
            slimeLabels = new List<SlimeContainer>();

            for (int i = 0; i < slimeContainers.Count; i++)
            {
                slimeLabels.Add(new SlimeContainer()
                {
                    username        = slimeContainers[i].Q("Username")        as Label,
                    posts           = slimeContainers[i].Q("Posts")           as Label,
                    followers       = slimeContainers[i].Q("Followers")       as Label,
                    following       = slimeContainers[i].Q("Following")       as Label,
                    lifetime        = slimeContainers[i].Q("Lifetime")        as Label,
                    population      = slimeContainers[i].Q("Population")      as Label,
                    alivePopulation = slimeContainers[i].Q("AlivePopulation") as Label,
                });
            }
        }

        public void updateSlimes(List<InstagramSlime> slimes)
        {
            for (int i = 0; i < slimeLabels.Count; i++)
            {
                if (i < slimes.Count)
                {
                    slimeLabels[i].username.text        = slimes[i].userName;
                    slimeLabels[i].username.style.color = slimes[i].species.color;
                    slimeLabels[i].posts.text           = "posts " + slimes[i].posts.ToString();
                    slimeLabels[i].followers.text       = "followers " + slimes[i].follower.ToString();
                    slimeLabels[i].following.text       = "following " + slimes[i].following.ToString();
                    slimeLabels[i].lifetime.text        = slimes[i].lifeTime.ToString("N2");
                    slimeLabels[i].population.text      = slimes[i].species.population.ToString();
                    slimeLabels[i].alivePopulation.text = slimes[i].species.alivePopulation.ToString();
                }
                else
                {
                    slimeLabels[i].username.text        = "EMPTY";
                    slimeLabels[i].username.style.color = Color.white;
                    slimeLabels[i].posts.text           = "//";
                    slimeLabels[i].followers.text       = "//";
                    slimeLabels[i].following.text       = "//";
                    slimeLabels[i].lifetime.text        = "//";
                    slimeLabels[i].population.text      = "//";
                    slimeLabels[i].alivePopulation.text = "//";
                }
            }
        }

        public void updateQueue(int sign)
        {
            if (sign > 0)
            {
                if (profileQueue.Count > 0)
                {
                    Label label = profileQueue.Dequeue();
                    queueElement.Remove(label);
                }
            }
        }
    }
}