using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Linq;
using UnityEngine.SceneManagement;

public class CommandsHandler : MonoBehaviour
{

    public statsSetter stats;

    public TMP_Text terminal_text;
    public TMP_InputField terminal_input;
    public GameObject BedroomOverlay;
    public GameObject BathroomOverlay;
    public GameObject KitchenOverlay;
    public GameObject OfficeOverlay;
    public GameObject GarageOverlay;

    public GameObject BedroomDoor;
    public GameObject GarageDoor;
    public GameObject KitchenDoor;

    public GameObject Sprinkler;
    public GameObject Stereo;
    public GameObject Shower;
    public GameObject Stove;

    public AudioSource light_switch;
    public AudioSource door_sound;

    Queue<string> commands = new Queue<string>();
    public int queue_pointer = 0;

    public GuyMovement guyMovement;
    public bool override_available = false;
    public GameObject Fire;
    public SpriteRenderer house;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        terminal_input = this.GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        bool pointer_changed = false;
        if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (queue_pointer - 1 < commands.Count && queue_pointer - 1 >= 0)
            {
                queue_pointer--;
                pointer_changed = true;
            }
        }
        if (UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if (queue_pointer + 1 <= commands.Count && queue_pointer + 1 >= 0)
            {
                queue_pointer++;
                pointer_changed = true;
            }
        }
        if (pointer_changed)
        {
            if (queue_pointer == commands.Count)
                terminal_input.text = "";
            else
                terminal_input.text = commands.ElementAt(queue_pointer);
        }
    }

    bool processCommand(string command)
    {
        // Implement your command processing logic here
        // Return true if the command was processed successfully, false otherwise

        if (command.Contains("help"))
        {
            terminal_text.text +=
             "\nAvailable commands:\n" +
             "- <color=#00FFFF>help</color>: Show this help message\n" +
             "- <color=#00FFFF>clear</color>: Clear the terminal\n" +
             "- <color=#00FFFF>echo [message]</color>: Echo the message back to the terminal\n\n" +

             "<color=#00FFFF>room.lights.on/off:</color> switch lights\n" +
             "<color=#00FFFF>room.door.open/close:</color> open/close door\n" +
             "<color=#00FFFF>garden.sprinkler.on/off:</color> water flower\n" +
             "<color=#00FFFF>bedroom.stereo.play(metal/mozart):</color> sound control\n" +
             "<color=#00FFFF>bedroom.stereo.off:</color> stereo off\n" +
             "<color=#00FFFF>bathroom.shower.on/off:</color> on/off shower\n" +
             "<color=#00FFFF>kitchen.stove.heat/cool:</color> stove control\n";

            return false;
        }
        else if (command.Contains("clear"))
        {
            terminal_text.text = "";
            return false;
        }
        else if (command.StartsWith("echo "))
        {
            string message = command.Substring(5); // Get the message after "echo "
            terminal_text.text += "\n" + message;
            return false;
        }
        else if (command.Contains("bedroom.lights.on"))
        {
            terminal_text.text += "\nTurning on bedroom lights...";
            return switch_lights(BedroomOverlay, true);
        }
        else if (command.Contains("bedroom.lights.off"))
        {
            terminal_text.text += "\nTurning off bedroom lights...";
            return switch_lights(BedroomOverlay, false);
        }
        else if (command.Contains("bathroom.lights.on"))
        {
            terminal_text.text += "\nTurning on bathroom lights...";
            return switch_lights(BathroomOverlay, true);
        }
        else if (command.Contains("bathroom.lights.off"))
        {
            terminal_text.text += "\nTurning off bathroom lights...";
            return switch_lights(BathroomOverlay, false);
        }
        else if (command.Contains("kitchen.lights.on"))
        {
            terminal_text.text += "\nTurning on kitchen lights...";
            return switch_lights(KitchenOverlay, true);
        }
        else if (command.Contains("kitchen.lights.off"))
        {
            terminal_text.text += "\nTurning off kitchen lights...";
            return switch_lights(KitchenOverlay, false);
        }
        else if (command.Contains("office.lights.on"))
        {
            terminal_text.text += "\nTurning on office lights...";
            return switch_lights(OfficeOverlay, true);
        }
        else if (command.Contains("office.lights.off"))
        {
            terminal_text.text += "\nTurning off office lights...";
            return switch_lights(OfficeOverlay, false);
        }
        else if (command.Contains("garage.lights.on"))
        {
            terminal_text.text += "\nTurning on garage lights...";
            return switch_lights(GarageOverlay, true);
        }
        else if (command.Contains("garage.lights.off"))
        {
            terminal_text.text += "\nTurning off garage lights...";
            return switch_lights(GarageOverlay, false);
        }
        else if (command.Contains("bedroom.door.open"))
        {
            terminal_text.text += "\nOpening bedroom door...";
            return open_close_door(BedroomDoor, true);
        }
        else if (command.Contains("bedroom.door.close"))
        {
            terminal_text.text += "\nClosing bedroom door...";
            return open_close_door(BedroomDoor, false);
        }
        else if (command.Contains("garage.door.open"))
        {
            terminal_text.text += "\nOpening garage door...";
            return open_close_door(GarageDoor, true);
        }
        else if (command.Contains("garage.door.close"))
        {
            terminal_text.text += "\nClosing garage door...";
            return open_close_door(GarageDoor, false);
        }
        else if (command.Contains("kitchen.door.open"))
        {
            terminal_text.text += "\nOpening kitchen door...";
            return open_close_door(KitchenDoor, true);
        }
        else if (command.Contains("kitchen.door.close"))
        {
            terminal_text.text += "\nClosing kitchen door...";
            return open_close_door(KitchenDoor, false);
        }
        else if (command.Contains("garden.sprinkler.on"))
        {
            terminal_text.text += "\nStarting garden sprinkler...";
            Sprinkler.GetComponent<Animator>().SetBool("watering", true);
            return true;
        }
        else if (command.Contains("garden.sprinkler.off"))
        {
            terminal_text.text += "\nStoping garden sprinkler...";
            Sprinkler.GetComponent<Animator>().SetBool("watering", false);
            return true;
        }
        else if (command.Contains("bedroom.stereo.play(metal)")) {
            terminal_text.text += "\nPlaying metal...";
            Stereo.GetComponent<PlayingStereo>().playMetal();
            return true;
        }
        else if (command.Contains("bedroom.stereo.play(mozart)"))
        {
            terminal_text.text += "\nPlaying mozart...";
            Stereo.GetComponent<PlayingStereo>().playMozart();
            return true;
        }
        else if (command.Contains("bedroom.stereo.off")) {
            terminal_text.text += "\nTurning off radio...";
            Stereo.GetComponent<PlayingStereo>().stopMusic();
            return true;
        }
        else if (command.Contains("bathroom.shower.on"))
        {
            terminal_text.text += "\nTurning on shower...";
            Shower.GetComponent<Animator>().SetBool("showerOn", true);
            Shower.GetComponent<AudioSource>().Play();
            return true;
        }
        else if (command.Contains("bathroom.shower.off"))
        {
            terminal_text.text += "\nTurning off shower...";
            Shower.GetComponent<Animator>().SetBool("showerOn", false);
            Shower.GetComponent<AudioSource>().Pause();
            return true;
        }
        else if (command.Contains("kitchen.stove.heat"))
        {
            terminal_text.text += "\nHeating up stove...";
            Stove.GetComponent<Animator>().SetBool("heatUp", true);
            return true;
        }
        else if (command.Contains("kitchen.stove.cool"))
        {
            terminal_text.text += "\nCooling down stove...";
            Stove.GetComponent<Animator>().SetBool("heatUp", false);
            return true;
        }
        else if (command.Contains("restart"))
        {
            SceneManager.LoadScene(0);
            return true;
        }
        else if (command.Contains("office.termostat.overheat") && override_available)
        {
            guyMovement.override_on = true;
            guyMovement.speed = 3;
            guyMovement.GetComponent<Animator>().SetFloat("Run", 3);
            open_close_door(GarageDoor, true);
            open_close_door(KitchenDoor, true);
            open_close_door(BedroomDoor, true);
            Fire.SetActive(true);
            house.color = new Color(1.0f, 93.0f/ 255.0f, 93.0f/255.0f);
            terminal_text.text += "\n<color=green>AIHouse</color> <color=red>OVERRIDE</color> with remaining <color=blue>" + stats.trust + "%</color> trust!";

            return true;
        }

        terminal_text.text += "\nUnknown command: <color=red>\"" + command + "\"</color>";
        return false;
    }

    public bool switch_lights(GameObject overlay, bool on)
    {
        if (overlay.activeSelf != on)
        {
            return false;
        }
        light_switch.Play();
        overlay.SetActive(!on);

        return true;
    }

    public bool open_close_door(GameObject door, bool open)
    {
        if (door.activeSelf == open)
        {
            return false;
        }
        door_sound.Play();
        door.SetActive(open);

        return true;
    }
    public void receiveCommand()
    {
        var command = gameObject.GetComponent<TMP_InputField>().text;
        gameObject.GetComponent<TMP_InputField>().text = "";
        terminal_text.text += "\n> " + command;
        commands.Enqueue(command);
        queue_pointer = commands.Count;
        if (processCommand(command))
        {
            if (stats.override_val < 100)
                stats.override_val++;
        }

        Debug.Log("Received command: " + command);
        // Process the command here
        StartCoroutine(Refocus());
    }

    System.Collections.IEnumerator Refocus()
    {
        yield return null; // wait 1 frame
        gameObject.GetComponent<TMP_InputField>().ActivateInputField();
        gameObject.GetComponent<TMP_InputField>().Select();
    }
}
