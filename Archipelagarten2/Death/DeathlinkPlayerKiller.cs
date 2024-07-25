using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Archipelagarten2.Constants;
using Archipelagarten2.UnityObjects;
using DG.Tweening;
using KG2;
using UnityEngine;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;
using Object = UnityEngine.Object;

namespace Archipelagarten2.Death
{
    internal class PlayerKiller
    {
        private Dictionary<string, Func<bool>> _killMethods;
        private Dictionary<string, int> _deathIds;

        private ILogger _logger;
        private UnityActions _unityActions;
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
                { "Wrong button.", () => DoDefaultDeath("Wrong button.") },
            };
        }

        private void InitializeDeathIds()
        {
            try
            {
                var deathPanel = _unityActions.FindOrCreatePanel<DeathPanel>();
                var deathMessages = DeathPanel.DeathMessages.LoadDeathMessage(deathPanel.deathXML);
                _deathIds = new Dictionary<string, int>();
                foreach (var message in deathMessages.Messages)
                {
                    if (_deathIds.ContainsKey(message.Message))
                    {
                        continue;
                    }

                    _deathIds.Add(message.Message, message.DeathIndex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarningException($"Could not initialize death ids", ex);
            }
        }

        public PlayerKiller(ILogger logger, UnityActions unityActions, bool deathLink)
        {
            _logger = logger;
            _unityActions = unityActions;
            _deathLink = deathLink;
            InitializeKillMethods();
        }

        public void KillInSpecificWay(string deathMessage)
        {
            try
            {
                if (_killMethods.ContainsKey(deathMessage))
                {
                    if (!_killMethods[deathMessage]())
                    {
                        DoDefaultDeath(deathMessage);
                    }
                }
                else
                {
                    throw new ArgumentException($"Unrecognized kill method: {deathMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarningException(ex, deathMessage);
                DoDefaultDeath(deathMessage);
            }
        }

        private bool DoDefaultDeath(string deathMessage)
        {
            if (_deathIds == null)
            {
                InitializeDeathIds();
            }

            if (_deathIds == null)
            {
                CallDeathUi(68);
                return false;
            }

            CallDeathUi(_deathIds[deathMessage]);
            return true;
        }

        private bool JanitorKillPlayer(int killId)
        {
            var janitor = _unityActions.FindOrCreateNpc<Janitor>();

            if (!_unityActions.MoveNPCToRangedDistance(janitor))
            {
                return false;
            }

            return JanitorKillPlayer(janitor, killId);
        }

        private bool CindyTellJanitorToKillPlayer()
        {
            var cindy = _unityActions.FindOrCreateNpc<Cindy>();
            if (!_unityActions.MoveNPCToRangedDistance(cindy))
            {
                return false;
            }

            var janitor = _unityActions.FindOrCreateNpc<Janitor>();

            CallPrivateMethod(cindy, "CallCops");
            CallPrivateMethod(cindy, "JanitorComeToClass");

            return JanitorKillPlayer(janitor, 44);
        }

        private bool SlipAndFall()
        {
            var janitor = _unityActions.FindOrCreateNpc<Janitor>();
            if (!_unityActions.MoveNPCToRangedDistance(janitor))
            {
                return false;
            }

            return CallDeathUi(janitor, 5);
        }

        private bool JanitorKillPlayer(Janitor janitor, int killId)
        {
            janitor.KillPlayer(GetOffsetKillId(killId));
            return true;
        }

        private bool JumpIntoHole()
        {
            var interactable = _unityActions.FindOrCreateInteractable();
            interactable.player.SetPlayerState(PlayerState.AnimState);
            interactable.player.SetAnimatorBool("IsJumping", true);

            interactable.player.WalkToPoint(new Vector3(-0.943f, -0.68f), 0.25f, () => JumpInHole(interactable));

            return true;
        }

        private void JumpInHole(ObjectInteractable interactable)
        {
            interactable.player.SetSpriteMasking(true);
            var nuggetHoleMask = _unityActions.FindOrCreateByName("NuggetHoleMask");
            nuggetHoleMask.GetComponent<SpriteMask>().enabled = true;
            // interactable.player.transform.DOLocalMoveY(-1.53f, 1.75f);

            interactable.StartCoroutine(JumpedIntoHole(interactable));
        }

        private IEnumerator JumpedIntoHole(ObjectInteractable interactable)
        {
            yield return new WaitForSeconds(4f);
            Object.FindObjectOfType<CameraController>().CameraShake(0.25f);
            CallDeathUi(interactable, 11);
        }

        private bool CrushBomb()
        {
            _logger.LogDebug($"{nameof(PlayerKiller)}.{nameof(CrushBomb)}");

            var interactable = _unityActions.FindOrCreateInteractable();
            AudioController.instance.PlaySound("AcidDoorOpen");
            var jeromeBombCrusher = _unityActions.FindOrCreateByName("JeromeBombCrusher");
            jeromeBombCrusher.GetComponent<SpriteRenderer>().enabled = false;
            interactable.player.Explode();
            interactable.player.SetPlayerState(PlayerState.AnimState);
            Object.FindObjectOfType<CameraController>().CameraShake(0.25f);
            CallDeathUi(interactable, 67, 3f);
            interactable.UI.CallDeath(DeathId.DEATHLINK_OFFSET + 67, 3f);
            var hydraulicPress = _unityActions.FindOrCreateByName("HydraulicPress");
            hydraulicPress.transform.localPosition = new Vector3(-1.885f, -20.64f, 0.0f);
            var hydraulicBaseDestroyed = _unityActions.FindOrCreateByName("HydraulicBaseDestroyed");
            hydraulicBaseDestroyed.transform.localPosition = new Vector3(-1.885f, -0.64f, 0.0f);
            var hydraulicBurst = _unityActions.FindOrCreateByName("HydraulicBurst");
            hydraulicBurst.GetComponent<ParticleSystem>().Play();

            return true;
        }

        private bool BillyGetSuckedIn()
        {
            var billy = _unityActions.FindOrCreateNpc<Billy>();
            var lily = billy.GetNPC("Lily");
            if (!_unityActions.MoveNPCToRangedDistance(billy))
            {
                return false;
            }

            if (!_unityActions.MoveNPCToRangedDistance(lily))
            {
                return false;
            }

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
            return CallDeathUi(billy, 31, 4f);
        }

        private bool LilyGetSuckedIn()
        {
            var lily = _unityActions.FindOrCreateNpc<Lily>();
            var billy = lily.GetNPC("Billy");
            if (!_unityActions.MoveNPCToRangedDistance(lily))
            {
                return false;
            }

            if (!_unityActions.MoveNPCToRangedDistance(billy))
            {
                return false;
            }

            lily.ClearCameraTarget();
            lily.player.SetPlayerState(PlayerState.AnimState);
            Object.FindObjectOfType<BeastBehavior>().SetAnimatorTrigger("StartInhale");
            var particleinhale = _unityActions.FindOrCreateByName("Particle Inhale");
            particleinhale.GetComponent<ParticleSystem>().Play();
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

            return CallDeathUi(billy, 32, 4f);
        }

        private bool CreatureKillNotDistracted() => CreatureKill(22);

        private bool CreatureKillTooClose() => CreatureKill(23);

        private bool CreatureKillLookingAtYou() => CreatureKill(63);

        private bool CreatureKillBeforeGoingForPrincipal() => CreatureKill(64);

        private bool PlayerWalkToMonster()
        {
            var playerHeadBloodSplatter = _unityActions.FindOrCreateByName("PlayerHeadBloodSplatter");
            playerHeadBloodSplatter.GetComponent<ParticleSystem>().Play();
            var playerController = _unityActions.FindOrCreatePlayer();
            var creature = _unityActions.FindOrCreateNpc<CreatureBehavior>();
            if (!_unityActions.MoveNPCToRangedDistance(creature))
            {
                return false;
            }

            var creatureAnimationEvents = _unityActions.FindOrCreate<CreatureAnimationEvents>();
            playerController.transform.SetParent(creatureAnimationEvents.transform.parent.parent);
            playerController.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, 0.0f), 0.2f);
            playerController.SetAnimatorTrigger("FallDown");
            var mStoreY = (float)typeof(CreatureAnimationEvents).GetField("mStoreY", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(creatureAnimationEvents);
            playerController.WalkToPoint(new Vector3(Mathf.Min(2.5f, creatureAnimationEvents.transform.parent.position.x + 1.2f), mStoreY), 0.4f);
            playerController.SetShadowEnabled(true);

            return true;
        }

        private bool CreatureKill(int killId)
        {
            if (!PlayerWalkToMonster())
            {
                return false;
            }

            if (!CallDeathUi(Object.FindObjectOfType<UIController>(), killId, 2f))
            {
                return false;
            }

            AudioController.instance.PlaySound("GoreSplat2");
            return true;
        }

        private bool ExplodePlayer()
        {
            var jerome = _unityActions.FindOrCreateNpc<Jerome>();
            if (!_unityActions.MoveNPCToRangedDistance(jerome))
            {
                return false;
            }

            EnvironmentController.Instance.UseItem(Item.JeromeBomb);
            jerome.player.SetPlayerState(PlayerState.AnimState);
            jerome.StartCoroutine(Beep3Times(jerome));
            return true;
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

        private bool FailSwingPuzzle()
        {
            var jerome = _unityActions.FindOrCreateNpc<Jerome>();
            if (!_unityActions.MoveNPCToRangedDistance(jerome))
            {
                return false;
            }

            jerome.player.Explode();
            jerome.player.SetPlayerState(PlayerState.AnimState);
            Object.FindObjectOfType<CameraController>().CameraShake(0.25f);
            return CallDeathUi(jerome, 34, 3);
        }

        private bool LunchLadyKillPlayerMorningTime() => LunchLadyKillPlayer(27);

        private bool LunchLadyKillPlayerCaughtWithRemote() => LunchLadyKillPlayer(48);

        private bool LunchLadyKillPlayerNuggetBetterSituation() => LunchLadyKillPlayer(51);

        private bool LunchLadyKillPlayerOutside() => LunchLadyKillPlayer(58);

        private bool LunchLadyKillPlayer(int killId)
        {
            var lunchLady = _unityActions.FindOrCreateNpc<LunchLady>();
            if (!_unityActions.MoveNPCToRangedDistance(lunchLady))
            {
                return false;
            }

            lunchLady.SetSpriteOrder(1);
            lunchLady.player.SetPlayerState(PlayerState.AnimState);
            lunchLady.GetComponentInChildren<AnimationEvents>().SetTargetCharacter("Player");
            lunchLady.SetAnimatorTrigger("IsAttacking");
            EnvironmentController.Instance.UnlockFullOutfit(21);
            return CallDeathUi(lunchLady, killId, 4f);
        }

        private bool ExplodeChemistry()
        {
            var monty = _unityActions.FindOrCreateNpc<Monty>();
            if (!_unityActions.MoveNPCToRangedDistance(monty))
            {
                return false;
            }

            EnvironmentController.Instance.UnlockFullOutfit(5);
            monty.player.SetPlayerState(PlayerState.AnimState);
            monty.player.GetBurned();
            monty.SetAnimatorTrigger("Fire");
            var gameObject = Object.Instantiate((GameObject)Resources.Load("Prefabs/particleFire"));
            gameObject.transform.parent = monty.transform;
            gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, -1f);
            var chemistrySetBurned = _unityActions.FindOrCreateByName("ChemistrySetBurned");
            chemistrySetBurned.transform.localPosition = new Vector3(-1.885f, -0.64f, 0.0f);
            var chemistrySet = _unityActions.FindOrCreateByName("ChemistrySet");
            chemistrySet.GetComponent<SpriteRenderer>().enabled = false;
            var chemistrySetFire = _unityActions.FindOrCreateByName("ChemistrySetFire");
            chemistrySetFire.GetComponent<ParticleSystem>().Play();
            var chemistrySetBurst = _unityActions.FindOrCreateByName("ChemistrySetBurst");
            chemistrySetBurst.GetComponent<ParticleSystem>().Play();
            Object.FindObjectOfType<CameraController>().CameraShake(0.25f);
            AudioController.instance.PlaySound("Explosion");
            AudioController.instance.PlaySound("Fire");
            return CallDeathUi(monty, 21, 3f);
        }

        public bool MontyCannonFireLeftUpstairs() => MontyKillCannon(6);

        public bool MontyCannonFireChemistry() => MontyKillCannon(20);

        public bool MontyCannonFireUpstairs() => MontyKillCannon(57);

        public bool MontyCannonFireElevatorKey() => MontyKillCannon(62);

        private bool MontyKillCannon(int killId)
        {
            var monty = _unityActions.FindOrCreateNpc<Monty>();
            if (!_unityActions.MoveNPCToRangedDistance(monty))
            {
                return false;
            }

            monty.SetCannonDeathIndex(GetOffsetKillId(killId));
            monty.StartCoroutine(OpenAndFireCannon(monty));

            return true;
        }

        private static IEnumerator OpenAndFireCannon(Monty monty)
        {
            monty.OpenCannon();
            yield return new WaitForSeconds(2f);
            monty.FireCannon();
        }

        public bool PlayerShotByPenny(int killId, bool turnOffLights = true)
        {
            var penny = _unityActions.FindOrCreateNpc<Penny>();
            if (!_unityActions.MoveNPCToRangedDistance(penny))
            {
                return false;
            }

            penny.StartCoroutine(PlayerShotInTheDark(penny, turnOffLights));
            return CallDeathUi(penny, killId, 3f);
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

        private bool OzzyStranglePlayer()
        {
            var ozzy = _unityActions.FindOrCreateNpc<Ozzy>();
            if (!_unityActions.MoveNPCToRangedDistance(ozzy))
            {
                return false;
            }

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

            return true;
        }

        public bool GetFaceEaten()
        {
            var playerController = _unityActions.FindOrCreatePlayer();

            playerController.ApplyFaceOverlay("Kid_Overlay_Eaten");
            playerController.GetComponentInChildren<Animator>().SetBool("Decapitation", true);
            playerController.SetPlayerState(PlayerState.AnimState);
            var playerHeadBloodStream = _unityActions.FindOrCreateByName("PlayerHeadBloodStream");
            playerHeadBloodStream.GetComponent<ParticleSystem>().Play();
            return CallDeathUi(10, 2f);
        }

        public bool GetElectrocuted()
        {
            var playerController = _unityActions.FindOrCreatePlayer();

            playerController.GetComponentInChildren<Animator>().SetBool("Electrocution", true);
            playerController.SetPlayerState(PlayerState.AnimState);
            return CallDeathUi(33, 3f);
        }

        public bool GetPoisoned()
        {
            var playerController = _unityActions.FindOrCreatePlayer();

            playerController.SetPlayerState(PlayerState.AnimState);
            playerController.GetComponentInChildren<Animator>().SetBool("Poison", true);
            return CallDeathUi(49, 3f);
        }

        private bool DieByBees()
        {
            var playerController = _unityActions.FindOrCreatePlayer();
            playerController.SetPlayerState(PlayerState.AnimState);
            return CallBees(playerController);
        }

        private bool CallBees(PlayerController playerController)
        {
            GameObject bees = _unityActions.FindOrCreateByName("BeesObject");
            var beesRoot = _unityActions.FindOrCreateByName("BeesRoot");
            beesRoot.GetComponent<Animator>().speed = 2f;
            AudioController.instance.PlaySound("Bees");
            bees.transform.DOMove(playerController.transform.position, 1f).OnComplete(() => GetStung(playerController));
            return true;
        }

        private bool GetStung(PlayerController playerController)
        {
            playerController.ApplyNewFace(EnvironmentController.Instance.saveFile.CurrentHairStyle, playerController.stungFace);
            playerController.GetComponentInChildren<Animator>().SetBool("Stung", true);
            playerController.StartCoroutine(StopBees());
            return true;
        }

        private IEnumerator StopBees()
        {
            yield return new WaitForSeconds(3f);
            AudioController.instance.StopSound("Bees");
            CallDeathUi(9);
        }

        private bool BuggsKillPlayerDodgeball()
        {
            var buggs = _unityActions.FindOrCreateNpc<Buggs>();
            if (!_unityActions.MoveNPCToRangedDistance(buggs))
            {
                return false;
            }

            buggs.player.SetPlayerState(PlayerState.AnimState);
            buggs.ThrowItem(buggs.player.transform.localPosition + new Vector3(0.0f, 0.08f), 0.3f, () =>
            {
                var dodgeballFloorMid = _unityActions.FindOrCreateByName("DodgeballFloorMid");
                dodgeballFloorMid.GetComponent<BoxCollider2D>().enabled = true;
                var dodgeballMiddle = _unityActions.FindOrCreateByName("DodgeballMiddle");
                dodgeballMiddle.GetComponent<CircleCollider2D>().enabled = false;
                HeadFlyOff(13, "DodgeballMiddle");
            });

            return true;
        }

        private bool CarlaThrowDodgeballAtPlayer()
        {
            var carla = _unityActions.FindOrCreateNpc<Carla>();
            if (!_unityActions.MoveNPCToRangedDistance(carla))
            {
                return false;
            }

            carla.player.SetPlayerState(PlayerState.AnimState);
            carla.ThrowItem(carla.player.transform.localPosition + new Vector3(0.0f, 0.08f), 0.3f, () =>
            {
                var dodgeballMiddle = _unityActions.FindOrCreateByName("DodgeballMiddle");
                dodgeballMiddle.GetComponent<CircleCollider2D>().enabled = false;
                HeadFlyOff(12, "DodgeballMiddle");
            });

            return true;
        }

        private bool NuggetThrowBallButPlayerInTheWay()
        {
            var nugget = _unityActions.FindOrCreateNpc<Nugget>();
            if (!_unityActions.MoveNPCToRangedDistance(nugget))
            {
                return false;
            }

            nugget.player.SetPlayerState(PlayerState.AnimState);
            var dodgeballFloorMid = _unityActions.FindOrCreateByName("DodgeballFloorMid");
            dodgeballFloorMid.GetComponent<BoxCollider2D>().enabled = false;
            var dodgeballFloorLow = _unityActions.FindOrCreateByName("DodgeballFloorLow");
            dodgeballFloorLow.GetComponent<BoxCollider2D>().enabled = true;
            nugget.ThrowItem(nugget.player.transform.localPosition + new Vector3(0.0f, 0.08f), 0.1f, () =>
            {
                var dodgeballBottom = _unityActions.FindOrCreateByName("DodgeballBottom");
                dodgeballBottom.GetComponent<CircleCollider2D>().enabled = false;
                HeadFlyOff(14, "DodgeballBottom", true);
            });

            return true;
        }

        public bool HeadFlyOff(int x, string ball)
        {
            var playerController = _unityActions.FindOrCreatePlayer();

            playerController.ApplyFaceOverlay("Kid_Overlay_Decapitation");
            AudioController.instance.PlaySound("GoreSplat1");
            GameObject gameObject = _unityActions.FindOrCreateByName("PlayerHeadDecapitated");
            playerController.GetDecapitated();
            gameObject.transform.position = playerController.head.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sprite = playerController.head.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-1f, 1.5f);
            gameObject.GetComponent<Rigidbody2D>().AddTorque(-1f);
            GameObject.Find(ball).GetComponent<SpriteRenderer>().enabled = false;
            playerController.headSpriteRenderer.sprite = GameObject.Find(ball).GetComponent<SpriteRenderer>().sprite;
            return CallDeathUi(x, 2f);
        }

        public bool HeadFlyOff(int x, string ball, bool dir)
        {
            var playerController = _unityActions.FindOrCreatePlayer();

            AudioController.instance.PlaySound("GoreSplat1");
            playerController.ApplyFaceOverlay("Kid_Overlay_Decapitation");
            playerController.SetDirection(false);
            GameObject gameObject = _unityActions.FindOrCreateByName("PlayerHeadDecapitated");
            playerController.GetDecapitated();
            gameObject.transform.position = playerController.head.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sprite = playerController.head.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(1f, 1.5f);
            gameObject.GetComponent<Rigidbody2D>().AddTorque(1f);
            GameObject.Find(ball).GetComponent<SpriteRenderer>().enabled = false;
            playerController.headSpriteRenderer.sprite = GameObject.Find(ball).GetComponent<SpriteRenderer>().sprite;
            return CallDeathUi(x, 2f);
        }

        public bool ShotByScienceTeacher(int killId)
        {
            var scienceTeacher = _unityActions.FindOrCreateNpc<ScienceTeacher>();

            if (!_unityActions.MoveNPCToRangedDistance(scienceTeacher))
            {
                return false;
            }

            scienceTeacher.player.SetPlayerState(PlayerState.AnimState);
            scienceTeacher.ClearCameraTarget();
            scienceTeacher.FacePlayer();
            scienceTeacher.GetComponentInChildren<Animator>().SetBool(nameof(ScienceTeacher.Shoot), true);
            scienceTeacher.GetComponentInChildren<AnimationEvents>().SetTargetCharacter("Player");
            scienceTeacher.GetComponentInChildren<Animator>().SetBool("Shoot", false);
            return CallDeathUi(scienceTeacher, killId, 3f);
        }

        private bool CallDeathUi(Interactable interactable, int killId)
        {
            return interactable != null && CallDeathUi(interactable.UI, killId);
        }

        private bool CallDeathUi(int killId)
        {
            return CallDeathUi(Object.FindObjectOfType<UIController>(), killId);
        }

        private bool CallDeathUi(UIController uiController, int killId)
        {
            uiController?.CallDeath(GetOffsetKillId(killId));
            return uiController != null;
        }

        private bool CallDeathUi(Interactable interactable, int killId, float delay)
        {
            return interactable != null && CallDeathUi(interactable.UI, killId, delay);
        }

        private bool CallDeathUi(int killId, float delay)
        {
            return CallDeathUi(Object.FindObjectOfType<UIController>(), killId, delay);
        }

        private bool CallDeathUi(UIController uiController, int killId, float delay)
        {
            uiController?.CallDeath(GetOffsetKillId(killId), delay);
            return uiController != null;
        }

        private int GetOffsetKillId(int killId)
        {
            var offset = (_deathLink ? DeathId.DEATHLINK_OFFSET : 0);
            return offset + killId;
        }

        private bool CallPrivateMethod<T>(T obj, string methodName)
        {
            try
            {
                var method = typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(obj, Array.Empty<object>());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(ex, typeof(T).FullName, methodName);
                return false;
            }
        }
    }
}
