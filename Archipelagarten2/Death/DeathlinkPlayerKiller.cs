using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Archipelagarten2.Characters;
using Archipelagarten2.Constants;
using DG.Tweening;
using KG2;
using UnityEngine;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;
using Object = UnityEngine.Object;

namespace Archipelagarten2.Death
{
    internal class PlayerKiller
    {
        private Dictionary<string, Action> _killMethods;

        private ILogger _logger;
        private CharacterActions _characterActions;
        private bool _deathLink;

        private void InitializeKillMethods()
        {
            _killMethods = new()
            {
                { "New school, same janitor.", () => JanitorKillPlayer(0) },
                { "Don't shake the tree if the bees are gonna kill you.", DieByBees },
                { "Danner wants flowers or a cadaver. He's not picky.", () => ShotByScienceTeacher(3) },
                { "Dr. Danner is serious when he says to get out of his classroom.", () => ShotByScienceTeacher(4) },
                { "Puddles are slippery.", SlipAndFall },
                { "Monty is pretty bitter about being left upstairs. Don't get too close to him and he'll leave you alone.", MontyCannonFireLeftUpstairs },
                { "You should use your time a bit better.", () => JanitorKillPlayer(7) },
                { "Ozzy can get back up with a vengeance after using his inhaler.", OzzyStranglePlayer },
                { "Angry bees are bad for your health.", DieByBees },
                { "The plant creature wants the flowers in a different order.", GetFaceEaten },
                { "It may be a fresh Nugget Cave, but it's just as deep as the last one.", JumpIntoHole },
                { "You should bring something to hit the ball back with.", CarlaThrowDodgeballAtPlayer },
                { "Get out of Buggs's line of sight if you want to keep your head.", BuggsKillPlayerDodgeball },
                { "Don't get in Nugget's way.", NuggetThrowBallButPlayerInTheWay },
                { "No moving or talking in study hall. Now would be a good time to eat an apple.", () => ShotByScienceTeacher(15) },
                { "Dr.Danner is in the bathroom. You shouldn't go there.", () => ShotByScienceTeacher(16) },
                { "You should get back outside before the bell rings.", () => ShotByScienceTeacher(17) },
                { "Get back in the classroom before the lights come back on.", () => ShotByScienceTeacher(18) },
                // { "Find a way to distract Penny before she realizes the locker is being broken into.", MethodCall },
                { "Don't let anyone kick Monty off of the chemistry set.", MontyCannonFireChemistry },
                { "That was the wrong thing to mix with.", ExplodeChemistry },
                { "The creature will kill you if he isn't distracted by something.", CreatureKillNotDistracted },
                { "Don't get too close to the creature.", CreatureKillTooClose },
                { "Jerome might be a little mad that you helped kill his father.", ExplodePlayer },
                { "Don't wait until Penny is approaching to leave the car on the stairs.", () => PlayerShotByPenny(25) },
                { "You should press a button that sends Penny away from you.", () => PlayerShotByPenny(26) },
                { "Don't go inside unless the lunch lady has been removed.", LunchLadyKillPlayerMorningTime },
                // { "You'll have to save Lily immediately after killing the first monster.", DoDefaultDeath },
                // { "Save Billy after saving Lily.", DoDefaultDeath },
                // { "Eliminate the principal before the cannon fires.", DoDefaultDeath },
                { "Don't let the monster suck you up. Use the robotic claws to stop her.", BillyGetSuckedIn },
                { "Use the claws to force the monster to look up at Nugget when he comes down.", LilyGetSuckedIn },
                { "Don't stick forks in the electrical outlet. This applies to real life too.", GetElectrocuted },
                { "That was the wrong order.", FailSwingPuzzle },
                { "Penny will shoot everyone in the head if she isn't distracted. Maybe something from earlier will help?", () => PlayerShotByPenny(35) },
                { "Don't wait around if you decide to help the janitor.", () => JanitorKillPlayer(36) },
                { "He really wants that key before lunch starts.", () => JanitorKillPlayer(37) },
                { "You're an accomplice. Go upstairs and help the geezer out.", () => JanitorKillPlayer(38) },
                { "You should get back to your spot in study hall before Dr. Danner finds you.", () => ShotByScienceTeacher(39) },
                { "Students aren't allowed in the teacher's lounge.", () => PlayerShotByPenny(40) },
                { "Those bodies need to be taken care of or else one of you is going to prison.", () => JanitorKillPlayer(41) },
                { "You should help the janitor finish up with those bodies.", () => JanitorKillPlayer(42) },
                { "If Penny finds the contraband by the tree, you're on the hook for it.", () => PlayerShotByPenny(43) },
                { "Use the toilet paper to distract the janitor before dealing with Cindy.", CindyTellJanitorToKillPlayer },
                { "Getting caught out of study hall is a great way to get sent to the principal's office.", () => PlayerShotByPenny(45) },
                { "Get the knife before you get close to Applegate.", () => PlayerShotByPenny(46) },
                // { "You should do what Billy says as long as you're holding a listening device.", DoDefaultDeath },
                { "Don't get caught with the remote. Get it to Lily and Billy before Penny completes her scan.", LunchLadyKillPlayerCaughtWithRemote },
                { "It's not safe to stay in the janitor's closet too long.", GetPoisoned },
                { "The principal is waiting for you downstairs. It's best to sneak out the way you came in.", () => PlayerShotByPenny(50) },
                { "Nugget's in a better situation than you right now.", LunchLadyKillPlayerNuggetBetterSituation },
                { "You should have delivered the declaration.", () => JanitorKillPlayer(52) },
                { "Find a way to escape the bathroom.Your screwdriver could help.", () => PlayerShotByPenny(53) },
                { "Don't get sent to the principal's office.", () => PlayerShotByPenny(54) },
                { "If Billy draws the lunch lady's attention, you're all in trouble.", () => PlayerShotByPenny(55) },
                { "Penny will find you if you have contraband on you when the bell rings.", () => PlayerShotByPenny(56) },
                { "It's probably best if you just don't go upstairs.", MontyCannonFireUpstairs },
                { "Don't stick around outside. Get inside before the bell rings.", LunchLadyKillPlayerOutside },
                { "Don't get caught in study hall if you don't have study hall.", () => ShotByScienceTeacher(59) },
                { "Sneak in through the principal's office to avoid getting caught.", () => PlayerShotByPenny(60) },
                { "Get out of the bathroom before the janitor notices you've clogged the toilet.", () => JanitorKillPlayer(61) },
                { "Make sure Monty gets his elevator key back.", MontyCannonFireElevatorKey },
                { "Kill the creature that's looking at you before doing anything else.", CreatureKillLookingAtYou },
                { "Make sure all of the creatures are dead before going for the principal.", CreatureKillBeforeGoingForPrincipal },
                { "It's probably best if you don't show up at all, if you don't have the hair samples.", () => MontyKillCannon(65) },
                { "Don't go back out there.", () => PlayerShotByPenny(66) },
                { "It's not a good idea to crush an active explosive device.", CrushBomb },
                { "Wrong button.", DoDefaultDeath },
            };
        }

        public PlayerKiller(ILogger logger, CharacterActions characterActions, bool deathLink)
        {
            _logger = logger;
            _characterActions = characterActions;
            _deathLink = deathLink;
            InitializeKillMethods();
        }

        public void KillInSpecificWay(string deathMessage)
        {
            try
            {
                if (_killMethods.ContainsKey(deathMessage))
                {
                    _killMethods[deathMessage]();
                }
                else
                {
                    throw new ArgumentException($"Unrecognized kill method: {deathMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarningException(ex, deathMessage);
                DoDefaultDeath();
            }
        }

        private void DoDefaultDeath()
        {
            CallDeathUi(68);
        }

        private void JanitorKillPlayer(int killId)
        {
            var janitor = Object.FindObjectOfType<Janitor>();

            _characterActions.MoveNPCToCurrentRoom(janitor);
            JanitorKillPlayer(janitor, killId);
        }

        private void CindyTellJanitorToKillPlayer()
        {
            var cindy = Object.FindObjectOfType<Cindy>();
            _characterActions.MoveNPCToCurrentRoom(cindy);

            var janitor = Object.FindObjectOfType<Janitor>();

            CallPrivateMethod(cindy, "CallCops");
            CallPrivateMethod(cindy, "JanitorComeToClass");

            JanitorKillPlayer(janitor, 44);
        }

        private void SlipAndFall()
        {
            var janitor = Object.FindObjectOfType<Janitor>();
            _characterActions.MoveNPCToCurrentRoom(janitor);
            CallDeathUi(janitor, 5);
        }

        private void JanitorKillPlayer(Janitor janitor, int killId)
        {
            janitor.KillPlayer(GetOffsetKillId(killId));
        }

        private void JumpIntoHole()
        {
            var objectInteractable = Object.FindObjectOfType<ObjectInteractable>();
            objectInteractable.player.SetPlayerState(PlayerState.AnimState);
            objectInteractable.player.SetAnimatorBool("IsJumping", true);
            objectInteractable.player.WalkToPoint(new Vector3(-0.943f, -0.68f), 0.25f, () =>
            {
                objectInteractable.player.SetSpriteMasking(true);
                GameObject.Find("NuggetHoleMask").GetComponent<SpriteMask>().enabled = true;
                // objectInteractable.player.transform.DOLocalMoveY(-1.53f, 1.75f);

                IEnumerator JumpedIntoHole()
                {
                    yield return new WaitForSeconds(4f);
                    Object.FindObjectOfType<CameraController>().CameraShake(0.25f);
                    CallDeathUi(objectInteractable, 11);
                }

                objectInteractable.StartCoroutine(JumpedIntoHole());
            });
        }

        private void CrushBomb()
        {
            _logger.LogDebug($"{nameof(PlayerKiller)}.{nameof(CrushBomb)}");

            var objectInteractable = Object.FindObjectOfType<ObjectInteractable>();
            AudioController.instance.PlaySound("AcidDoorOpen");
            GameObject.Find("JeromeBombCrusher").GetComponent<SpriteRenderer>().enabled = false;
            objectInteractable.player.Explode();
            objectInteractable.player.SetPlayerState(PlayerState.AnimState);
            Object.FindObjectOfType<CameraController>().CameraShake(0.25f);
            CallDeathUi(objectInteractable, 67, 3f);
            objectInteractable.UI.CallDeath(DeathId.DEATHLINK_OFFSET + 67, 3f);
            GameObject.Find("HydraulicPress").transform.localPosition = new Vector3(-1.885f, -20.64f, 0.0f);
            GameObject.Find("HydraulicBaseDestroyed").transform.localPosition = new Vector3(-1.885f, -0.64f, 0.0f);
            GameObject.Find("HydraulicBurst").GetComponent<ParticleSystem>().Play();
        }

        private void BillyGetSuckedIn()
        {
            var billy = Object.FindObjectOfType<Billy>();
            var lily = billy.GetNPC("Lily");
            _characterActions.MoveNPCToCurrentRoom(billy);
            _characterActions.MoveNPCToCurrentRoom(lily);
            billy.ClearCameraTarget();
            billy.overrideMovementCode = true;
            billy.EnableSpriteMasks();
            billy.SetSpriteOrder(9);
            billy.HideShadow();
            billy.SetAnimatorBool("Flail", true);
            billy.WalkStraightLine(new Vector3(-4f, -0.34f), 2f);
            billy.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, -90f), 0.25f).SetEase(Ease.Linear);
            lily.overrideMovementCode = true;
            lily.EnableSpriteMasks();
            lily.SetSpriteOrder(9);
            lily.HideShadow();
            lily.SetAnimatorBool("Flail", true);
            lily.WalkStraightLine(new Vector3(-4f, -0.54f), 2.5f);
            lily.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, -90f), 0.25f).SetEase(Ease.Linear);
            billy.player.SetOverrideMovement(true);
            billy.player.SetSpriteMasking(true);
            billy.player.SetSpriteLayer(9);
            billy.player.SetShadowEnabled(false);
            billy.player.SetAnimatorBool("Flail", true);
            billy.player.transform.DOLocalMove(new Vector3(-4f, -0.54f), 3f).SetEase(Ease.Linear);
            billy.player.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, -90f), 0.25f).SetEase(Ease.Linear);
            Object.FindObjectOfType<BeastBehavior>().SetAnimatorTrigger("StopInhale");
            CallDeathUi(billy, 31, 4f);
        }

        private void LilyGetSuckedIn()
        {
            var lily = Object.FindObjectOfType<Lily>();
            var billy = lily.GetNPC("Billy");
            _characterActions.MoveNPCToCurrentRoom(lily);
            _characterActions.MoveNPCToCurrentRoom(billy);
            lily.ClearCameraTarget();
            lily.player.SetPlayerState(PlayerState.AnimState);
            Object.FindObjectOfType<BeastBehavior>().SetAnimatorTrigger("StartInhale");
            GameObject.Find("Particle Inhale").GetComponent<ParticleSystem>().Play();
            AudioController.instance.PlaySound("BeastSuck");
            lily.overrideMovementCode = true;
            lily.EnableSpriteMasks();
            lily.SetSpriteOrder(9);
            lily.HideShadow();
            lily.SetAnimatorBool("Flail", true);
            lily.WalkStraightLine(new Vector3(-4f, -0.34f), 2f);
            lily.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, -90f), 0.25f).SetEase(Ease.Linear);
            billy.overrideMovementCode = true;
            billy.EnableSpriteMasks();
            billy.SetSpriteOrder(9);
            billy.HideShadow();
            billy.SetAnimatorBool("Flail", true);
            billy.WalkStraightLine(new Vector3(-4f, 0.0f), 2.5f);
            billy.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, -90f), 0.25f).SetEase(Ease.Linear);
            lily.player.SetOverrideMovement(true);
            lily.player.SetSpriteMasking(true);
            lily.player.SetSpriteLayer(9);
            lily.player.SetShadowEnabled(false);
            lily.player.SetAnimatorBool("Flail", true);
            lily.player.transform.DOLocalMove(new Vector3(-4f, -0.54f), 3f).SetEase(Ease.Linear);
            lily.player.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, -90f), 0.25f).SetEase(Ease.Linear);
            Object.FindObjectOfType<BeastBehavior>().SetAnimatorTrigger("StopInhale", 4f);
            CallDeathUi(billy, 32, 4f);
        }

        private void CreatureKillNotDistracted()
        {
            PlayerWalkToMonster();
            CallDeathUi(Object.FindObjectOfType<UIController>(), 22, 2f);
            AudioController.instance.PlaySound("GoreSplat2");
        }

        private void CreatureKillTooClose()
        {
            PlayerWalkToMonster();
            CallDeathUi(Object.FindObjectOfType<UIController>(), 23, 2f);
            AudioController.instance.PlaySound("GoreSplat2");
        }

        private void CreatureKillLookingAtYou()
        {
            PlayerWalkToMonster();
            CallDeathUi(Object.FindObjectOfType<UIController>(), 63, 2f);
            AudioController.instance.PlaySound("GoreSplat2");
        }

        private void CreatureKillBeforeGoingForPrincipal()
        {
            PlayerWalkToMonster();
            CallDeathUi(Object.FindObjectOfType<UIController>(), 64, 2f);
            AudioController.instance.PlaySound("GoreSplat2");
        }

        private void PlayerWalkToMonster()
        {
            GameObject.Find("PlayerHeadBloodSplatter").GetComponent<ParticleSystem>().Play();
            var playerController = Object.FindObjectOfType<PlayerController>();
            var creature = Object.FindObjectOfType<CreatureBehavior>();
            _characterActions.MoveNPCToCurrentRoom(creature);
            var creatureAnimationEvents = Object.FindObjectOfType<CreatureAnimationEvents>();
            playerController.transform.SetParent(creatureAnimationEvents.transform.parent.parent);
            playerController.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, 0.0f), 0.2f);
            playerController.SetAnimatorTrigger("FallDown");
            var mStoreY = (float)typeof(CreatureAnimationEvents).GetField("mStoreY", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(creatureAnimationEvents);
            playerController.WalkToPoint(new Vector3(Mathf.Min(2.5f, creatureAnimationEvents.transform.parent.position.x + 1.2f), mStoreY), 0.4f);
            playerController.SetShadowEnabled(true);
        }

        private void ExplodePlayer()
        {
            var jerome = Object.FindObjectOfType<Jerome>();
            _characterActions.MoveNPCToCurrentRoom(jerome);
            EnvironmentController.Instance.UseItem(Item.JeromeBomb);
            jerome.player.SetPlayerState(PlayerState.AnimState);
            jerome.StartCoroutine(Beep3Times(jerome));
        }

        private IEnumerator Beep3Times(Jerome jerome)
        {
            for (var i = 0; i < 3; ++i)
            {
                AudioController.instance.PlaySound("Beep");
                yield return new WaitForSeconds(0.5f);
            }

            jerome.player.Explode();
            jerome.player.SetPlayerState(PlayerState.AnimState);
            Object.FindObjectOfType<CameraController>().CameraShake(0.25f);
            CallDeathUi(jerome, 24, 3);
        }

        private void FailSwingPuzzle()
        {
            var jerome = Object.FindObjectOfType<Jerome>();
            _characterActions.MoveNPCToCurrentRoom(jerome);
            jerome.player.Explode();
            jerome.player.SetPlayerState(PlayerState.AnimState);
            Object.FindObjectOfType<CameraController>().CameraShake(0.25f);
            CallDeathUi(jerome, 34, 3);
        }

        private void LunchLadyKillPlayerMorningTime()
        {
            LunchLadyKillPlayer(27);
        }

        private void LunchLadyKillPlayerCaughtWithRemote()
        {
            LunchLadyKillPlayer(48);
        }

        private void LunchLadyKillPlayerNuggetBetterSituation()
        {
            LunchLadyKillPlayer(51);
        }

        private void LunchLadyKillPlayerOutside()
        {
            LunchLadyKillPlayer(58);
        }

        private void LunchLadyKillPlayer(int killId)
        {
            var lunchLady = Object.FindObjectOfType<LunchLady>();
            _characterActions.MoveNPCToCurrentRoom(lunchLady);

            lunchLady.SetSpriteOrder(1);
            lunchLady.player.SetPlayerState(PlayerState.AnimState);
            lunchLady.GetComponentInChildren<AnimationEvents>().SetTargetCharacter("Player");
            lunchLady.SetAnimatorTrigger("IsAttacking");
            CallDeathUi(lunchLady, killId, 4f);
            EnvironmentController.Instance.UnlockFullOutfit(21);
        }

        private void ExplodeChemistry()
        {
            var monty = Object.FindObjectOfType<Monty>();
            _characterActions.MoveNPCToCurrentRoom(monty);

            EnvironmentController.Instance.UnlockFullOutfit(5);
            monty.player.SetPlayerState(PlayerState.AnimState);
            monty.player.GetBurned();
            monty.SetAnimatorTrigger("Fire");
            var gameObject = Object.Instantiate((GameObject)Resources.Load("Prefabs/particleFire"));
            gameObject.transform.parent = monty.transform;
            gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, -1f);
            GameObject.Find("ChemistrySetBurned").transform.localPosition = new Vector3(-1.885f, -0.64f, 0.0f);
            GameObject.Find("ChemistrySet").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("ChemistrySetFire").GetComponent<ParticleSystem>().Play();
            GameObject.Find("ChemistrySetBurst").GetComponent<ParticleSystem>().Play();
            Object.FindObjectOfType<CameraController>().CameraShake(0.25f);
            AudioController.instance.PlaySound("Explosion");
            AudioController.instance.PlaySound("Fire");
            CallDeathUi(monty, 21, 3f);
        }

        public void MontyCannonFireLeftUpstairs()
        {
            MontyKillCannon(6);
        }

        public void MontyCannonFireChemistry()
        {
            MontyKillCannon(20);
        }

        public void MontyCannonFireUpstairs()
        {
            MontyKillCannon(57);
        }

        public void MontyCannonFireElevatorKey()
        {
            MontyKillCannon(62);
        }

        private void MontyKillCannon(int killId)
        {
            var monty = Object.FindObjectOfType<Monty>();
            _characterActions.MoveNPCToCurrentRoom(monty);

            monty.SetCannonDeathIndex(GetOffsetKillId(killId));
            monty.StartCoroutine(OpenAndFireCannon(monty));
        }

        private static IEnumerator OpenAndFireCannon(Monty monty)
        {
            monty.OpenCannon();
            yield return new WaitForSeconds(2f);
            monty.FireCannon();
        }

        public void PlayerShotByPenny(int killId, bool turnOffLights = true)
        {
            var penny = Object.FindObjectOfType<Penny>();
            _characterActions.MoveNPCToCurrentRoom(penny);

            penny.StartCoroutine(PlayerShotInTheDark(penny, turnOffLights));
            CallDeathUi(penny, killId, 3f);
        }

        private static IEnumerator PlayerShotInTheDark(Penny penny, bool turnOffLights)
        {
            if (turnOffLights)
            {
                AudioController.instance.PlaySound("PowerDown");
                EnvironmentController.Instance.GetCurrentRoomEventManager().SetLights(false);
                RenderSettings.ambientLight = Color.black;
                yield return new WaitForSeconds(1f);
            }

            penny.player.SetPlayerState(PlayerState.DeadState);
            Object.FindObjectOfType<PlayerController>().GetShot();
            AudioController.instance.PlaySound("RayGun");

            yield return new WaitForSeconds(1f);
            RenderSettings.ambientLight = new Color(0.2265f, 0.2265f, 0.2265f);
            EnvironmentController.Instance.GetCurrentRoomEventManager().SetLights(true);
        }

        private void OzzyStranglePlayer()
        {
            var ozzy = Object.FindObjectOfType<Ozzy>();
            _characterActions.MoveNPCToCurrentRoom(ozzy);

            ozzy.dontTurnAround = false;
            ozzy.player.SetPlayerState(PlayerState.AnimState);
            var vector3 = new Vector3(-0.122f, -0.008f);
            if (ozzy.transform.localPosition.x > (double)ozzy.player.transform.localPosition.x)
            {
                vector3 = new Vector3(0.122f, -0.008f);
            }
            
            ozzy.SetFacialExpression(FacialExpression.Angry);
            ozzy.WalkToPoint(ozzy.player.transform.localPosition + vector3, 0.0f, () =>
            {
                ozzy.FacePlayer();
                ozzy.SetAnimatorBool("Strangle", true);
                ozzy.player.SetAnimatorBool("BeingStrangled", true);
                ozzy.SetAnimatorBool("Strangle", false, 2f);
                ozzy.player.SetAnimatorBool("BeingStrangled", false, 2f);
                ozzy.player.headSpriteRenderer.DOColor(new Color(0.8f, 0.8f, 1f), 1f);
                CallDeathUi(ozzy, 8, 4f);
            });
        }

        public void GetFaceEaten()
        {
            var playerController = Object.FindObjectOfType<PlayerController>();

            playerController.ApplyFaceOverlay("Kid_Overlay_Eaten");
            playerController.GetComponentInChildren<Animator>().SetBool("Decapitation", true);
            playerController.SetPlayerState(PlayerState.AnimState);
            GameObject.Find("PlayerHeadBloodStream").GetComponent<ParticleSystem>().Play();
            CallDeathUi(10, 2f);
        }

        public void GetElectrocuted()
        {
            var playerController = Object.FindObjectOfType<PlayerController>();

            playerController.GetComponentInChildren<Animator>().SetBool("Electrocution", true);
            playerController.SetPlayerState(PlayerState.AnimState);
            CallDeathUi(33, 3f);
        }

        public void GetPoisoned()
        {
            var playerController = Object.FindObjectOfType<PlayerController>();

            playerController.SetPlayerState(PlayerState.AnimState);
            playerController.GetComponentInChildren<Animator>().SetBool("Poison", true);
            CallDeathUi(49, 3f);
        }

        private void DieByBees()
        {
            var playerController = Object.FindObjectOfType<PlayerController>();

            GameObject bees = GameObject.Find("BeesObject");
            GameObject.Find("BeesRoot").GetComponent<Animator>().speed = 2f;
            AudioController.instance.PlaySound("Bees");
            bees.transform.DOMove(playerController.transform.position, 1f).OnComplete(() => GetStung(playerController));
        }

        private void GetStung(PlayerController playerController)
        {
            playerController.ApplyNewFace(EnvironmentController.Instance.saveFile.CurrentHairStyle, playerController.stungFace);
            playerController.GetComponentInChildren<Animator>().SetBool("Stung", true);
            playerController.StartCoroutine(StopBees());
        }

        private IEnumerator StopBees()
        {
            yield return new WaitForSeconds(3f);
            AudioController.instance.StopSound("Bees");
            CallDeathUi(9);
        }

        private void BuggsKillPlayerDodgeball()
        {
            var buggs = Object.FindObjectOfType<Buggs>();
            _characterActions.MoveNPCToCurrentRoom(buggs);

            buggs.player.SetPlayerState(PlayerState.AnimState);
            buggs.ThrowItem(buggs.player.transform.localPosition + new Vector3(0.0f, 0.08f), 0.3f, () =>
            {
                GameObject.Find("DodgeballFloorMid").GetComponent<BoxCollider2D>().enabled = true;
                GameObject.Find("DodgeballMiddle").GetComponent<CircleCollider2D>().enabled = false;
                HeadFlyOff(13, "DodgeballMiddle");
            });
        }

        private void CarlaThrowDodgeballAtPlayer()
        {
            var carla = Object.FindObjectOfType<Carla>();
            _characterActions.MoveNPCToCurrentRoom(carla);

            carla.player.SetPlayerState(PlayerState.AnimState);
            carla.ThrowItem(carla.player.transform.localPosition + new Vector3(0.0f, 0.08f), 0.3f, () =>
            {
                GameObject.Find("DodgeballMiddle").GetComponent<CircleCollider2D>().enabled = false;
                HeadFlyOff(12, "DodgeballMiddle");
            });
        }

        private void NuggetThrowBallButPlayerInTheWay()
        {
            var nugget = Object.FindObjectOfType<Nugget>();
            _characterActions.MoveNPCToCurrentRoom(nugget);

            nugget.player.SetPlayerState(PlayerState.AnimState);
            GameObject.Find("DodgeballFloorMid").GetComponent<BoxCollider2D>().enabled = false;
            GameObject.Find("DodgeballFloorLow").GetComponent<BoxCollider2D>().enabled = true;
            nugget.ThrowItem(nugget.player.transform.localPosition + new Vector3(0.0f, 0.08f), 0.1f, () =>
            {
                GameObject.Find("DodgeballBottom").GetComponent<CircleCollider2D>().enabled = false;
                HeadFlyOff(14, "DodgeballBottom", true);
            });
        }

        public void HeadFlyOff(int x, string ball)
        {
            var playerController = Object.FindObjectOfType<PlayerController>();

            playerController.ApplyFaceOverlay("Kid_Overlay_Decapitation");
            AudioController.instance.PlaySound("GoreSplat1");
            GameObject gameObject = GameObject.Find("PlayerHeadDecapitated");
            playerController.GetDecapitated();
            gameObject.transform.position = playerController.head.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sprite = playerController.head.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-1f, 1.5f);
            gameObject.GetComponent<Rigidbody2D>().AddTorque(-1f);
            GameObject.Find(ball).GetComponent<SpriteRenderer>().enabled = false;
            playerController.headSpriteRenderer.sprite = GameObject.Find(ball).GetComponent<SpriteRenderer>().sprite;
            CallDeathUi(x, 2f);
        }

        public void HeadFlyOff(int x, string ball, bool dir)
        {
            var playerController = Object.FindObjectOfType<PlayerController>();

            AudioController.instance.PlaySound("GoreSplat1");
            playerController.ApplyFaceOverlay("Kid_Overlay_Decapitation");
            playerController.SetDirection(false);
            GameObject gameObject = GameObject.Find("PlayerHeadDecapitated");
            playerController.GetDecapitated();
            gameObject.transform.position = playerController.head.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sprite = playerController.head.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(1f, 1.5f);
            gameObject.GetComponent<Rigidbody2D>().AddTorque(1f);
            GameObject.Find(ball).GetComponent<SpriteRenderer>().enabled = false;
            playerController.headSpriteRenderer.sprite = GameObject.Find(ball).GetComponent<SpriteRenderer>().sprite;
            CallDeathUi(x, 2f);
        }

        public void ShotByScienceTeacher(int killId)
        {
            var scienceTeacher = Object.FindObjectOfType<ScienceTeacher>();
            _characterActions.MoveNPCToCurrentRoom(scienceTeacher);

            scienceTeacher.player.SetPlayerState(PlayerState.AnimState);
            scienceTeacher.ClearCameraTarget();
            scienceTeacher.FacePlayer();
            scienceTeacher.GetComponentInChildren<Animator>().SetBool(nameof(ScienceTeacher.Shoot), true);
            scienceTeacher.GetComponentInChildren<AnimationEvents>().SetTargetCharacter("Player");
            CallDeathUi(scienceTeacher, killId, 3f);
            scienceTeacher.GetComponentInChildren<Animator>().SetBool("Shoot", false);
        }

        private void CallDeathUi(Interactable interactable, int killId)
        {
            CallDeathUi(interactable.UI, killId);
        }

        private void CallDeathUi(int killId)
        {
            CallDeathUi(Object.FindObjectOfType<UIController>(), killId);
        }

        private void CallDeathUi(UIController uiController, int killId)
        {
            uiController.CallDeath(GetOffsetKillId(killId));
        }

        private void CallDeathUi(Interactable interactable, int killId, float delay)
        {
            CallDeathUi(interactable.UI, killId, delay);
        }

        private void CallDeathUi(int killId, float delay)
        {
            CallDeathUi(Object.FindObjectOfType<UIController>(), killId, delay);
        }

        private void CallDeathUi(UIController uiController, int killId, float delay)
        {
            uiController.CallDeath(GetOffsetKillId(killId), delay);
        }

        private int GetOffsetKillId(int killId)
        {
            var offset = (_deathLink ? DeathId.DEATHLINK_OFFSET : 0);
            return offset + killId;
        }

        private void CallPrivateMethod<T>(T obj, string methodName)
        {
            try
            {
                var method = typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(obj, Array.Empty<object>());
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(ex, typeof(T).FullName, methodName);
            }
        }
    }
}
