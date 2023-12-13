using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

	public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [HideInInspector]
        public int id;

        public Player photonPlayer;

        public Rigidbody rig;

        private Animator anim;
        private CharacterController controller;

        public float speed = 600.0f;
        public float turnSpeed = 400.0f;
        private Vector3 moveDirection = Vector3.zero;
        public float gravity = 20.0f;

        private float startTime;
        private float timeTaken;

        private int collectabledPicked;
        public int maxCollectables = 10;

        //private bool isPlaying;

        //local player
        public static PlayerController me;

        void Awake()
        {
            rig = GetComponent<Rigidbody>();
        }

        [PunRPC]
        public void Initialize(Player player)
        {
            id = player.ActorNumber;
            photonPlayer = player;

            GameManager.instance.players[id - 1] = this;

            //is this not our local player?
            if (!photonView.IsMine)
            {
                GetComponentInChildren<Camera>().gameObject.SetActive(false);
                rig.isKinematic = true;
            }
        }


        void Start()
        {
            controller = GetComponent<CharacterController>();
            anim = gameObject.GetComponentInChildren<Animator>();

            startTime = Time.time;
            //isPlaying = true;
        }

        void Update()
        {
            if (!photonView.IsMine)
                return;

            Move();
        }

        void Move()
        {
            if (Input.GetKey("w"))
            {
                anim.SetInteger("AnimationPar", 1);
            }
            else
            {
                anim.SetInteger("AnimationPar", 0);
            }

            if (controller.isGrounded)
            {
                moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
            }

            float turn = Input.GetAxis("Horizontal");
            transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
            controller.Move(moveDirection * Time.deltaTime);
            moveDirection.y -= gravity * Time.deltaTime;
        }

        void End()
        {
            timeTaken = Time.time - startTime;
            Leaderboard.instance.SetLeaderboardEntry(-Mathf.RoundToInt(timeTaken * 1000.0f));
            //isPlaying = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Collectable"))
            {
                collectabledPicked++;
                Destroy(other.gameObject);

                if (collectabledPicked == maxCollectables)
                    End();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //stream.SendNext(curTimeText);
            }
        }
    }