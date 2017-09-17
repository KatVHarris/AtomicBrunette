using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform SecPointer, MinPointer, HourPointer, AlarmPointer, Hammer;
    public bool PlayPointerSound = true, AlarmActive = false, SystemDate = false, PlayingSound = false, Alarming = false;
    public int Year = 2011, Month = 4, Day = 11, Hour = 08, Minute = 30, Second = 0, AlarmHour = 10, AlarmMin = 0;
    public AudioSource AlarmAudio;
    public System.DateTime CustomClock, Alarm;
    public float speed = 1.0f;
    //
    private float sSecond = 0.0f, sMinute = 0.0f, sHour = 0.0f,sAlarmH,sAlarmM;
    private System.DateTime currentTime;
    private AudioSource Asource;
    //For CustomEditor
    public int NewYear, NewMonth, NewDay, NewHour, NewMinute, NewSecond;
    public Color NewColor = Color.white;
    public string ClockType = null;

    void Start()
    {
        Alarm = new System.DateTime(1,1,1,AlarmHour,AlarmMin,0);
        sAlarmM = Alarm.Minute;
        sAlarmH = Alarm.Hour % 12;
        
        if (!SystemDate)
        {
            CustomClock = new System.DateTime(NewYear, NewMonth, NewDay, NewHour, NewMinute, NewSecond);
        }
        if (ClockType != "Clock2")
        {
            Asource = GetComponent<AudioSource>();

        }
    }

    IEnumerator PlaySound()
    {
        Asource.Play();
        yield return new WaitForSeconds(1 / speed);
        PlayingSound = false;
    }

    void VerifyAlarm()
    {
        if (Alarming && AlarmActive)
        {
            double ang = (float)Mathf.Sin(Time.time * 100.0f) * 20;
            Hammer.localRotation = Quaternion.Euler(0, 0, (float)ang);
            AlarmAudio.enabled = true;
        }
        else
        {
            Alarming = false;
            AlarmAudio.enabled = false;
        }
    }

    private void SystemClock()
    {

        //
        if (SystemDate)
        {
            speed = 1.0f;
            currentTime = System.DateTime.Now;
            sSecond = System.DateTime.Now.Second;
            sMinute = System.DateTime.Now.Minute;
            sHour = System.DateTime.Now.Hour % 12;
            sSecond = System.DateTime.Now.Second;
            sMinute = System.DateTime.Now.Minute;
            sHour = System.DateTime.Now.Hour % 12;
            //
            Year = currentTime.Year;
            Month = currentTime.Month;
            Day = currentTime.Day;
            Hour = currentTime.Hour;
            Minute = currentTime.Minute;
            Second = currentTime.Second;
            //
        }
        else
        {
            CustomClock = CustomClock.AddSeconds(Time.deltaTime * speed);
            sSecond = CustomClock.Second;
            sMinute = CustomClock.Minute;
            sHour = CustomClock.Hour % 12;
            //
            Year = CustomClock.Year;
            Month = CustomClock.Month;
            Day = CustomClock.Day;
            Hour = CustomClock.Hour;
            Minute = CustomClock.Minute;
            Second = CustomClock.Second;
            currentTime = CustomClock;
        }
        if (PlayPointerSound && ClockType!="Clock2")
        {
            if (PlayingSound == false)
            {
                PlayingSound = true;
                StartCoroutine(PlaySound());
            }
        }
        if (ClockType == "Clock2") { 
            if (PlayPointerSound){
                transform.Find("ColSec").gameObject.SetActive(true);
            }else
            {
                transform.Find("ColSec").gameObject.SetActive(false);
            }
        }

        float SecondAngle = 360 * sSecond / 60;
        float minuteAngle = 360 * (sMinute / 60);
        float hourAngle = (360 * (sHour / 12))+(sMinute/2);
        float AlarmAngle = (360 * (sAlarmH /12))+(sAlarmM/2);
        if (ClockType != "Clock2")
        {
            SecPointer.localRotation = Quaternion.Euler(0, 0, SecondAngle);
        }
        else
        {
            double ang = (float)Mathf.Sin(Time.time * (speed * 2f)) * 100;
            SecPointer.localRotation = Quaternion.Euler(0, 0, (float)ang);
        }
        MinPointer.localRotation = Quaternion.Euler(0, 0, minuteAngle);
        HourPointer.localRotation = Quaternion.Euler(0, 0, hourAngle);
        //
        if(ClockType == "Clock3") { 
            AlarmPointer.localRotation = Quaternion.Euler(0,0,AlarmAngle);
        
        //
        if (AlarmActive)
        {
            if (sHour == sAlarmH && sMinute == sAlarmM)
            {
                Alarming = true;
            }
        }
        }
        //
        AlarmHour = Mathf.Clamp(AlarmHour,0, 24);
        AlarmMin = Mathf.Clamp(AlarmMin, 0, 60);
        Day = Mathf.Clamp(Day, 1, 31);
    }

  //Set a new Color
  public void SetNewColor()
    {
//        switch (clocktype)
//        {
//            case "Clock3":
                Renderer rendBell1 = transform.Find("Bell1").GetComponent<Renderer>();
                Renderer rendBell2 = transform.Find("Bell2").GetComponent<Renderer>();
                Renderer rendBase1 = transform.Find("Base1").GetComponent<Renderer>();
                Renderer rendBase2 = transform.Find("Base2").GetComponent<Renderer>();
                Renderer rendHammer = transform.Find("Hammer").GetComponent<Renderer>();
                Renderer rendhandle = transform.Find("handle").GetComponent<Renderer>();
                Renderer rendPin1 = transform.Find("Pin1").GetComponent<Renderer>();
                Renderer rendPin2 = transform.Find("Pin2").GetComponent<Renderer>();
                Renderer rendCenter = GetComponent<Renderer>();
                Material mat = new Material(rendBell1.sharedMaterial);
                mat.SetColor("_SpecColor", NewColor);
                rendBell1.material = mat;
                //
                mat = new Material(rendBell2.sharedMaterial);
                mat.SetColor("_SpecColor", NewColor);
                rendBell2.material = mat;
                //
                mat = new Material(rendBase1.sharedMaterial);
                mat.SetColor("_SpecColor", NewColor);
                rendBase1.material = mat;
                //
                mat = new Material(rendBase2.sharedMaterial);
                mat.SetColor("_SpecColor", NewColor);
                rendBase2.material = mat;
                //
                mat = new Material(rendHammer.sharedMaterial);
                mat.SetColor("_SpecColor", NewColor);
                rendHammer.material = mat;
                //
                mat = new Material(rendhandle.sharedMaterial);
                mat.SetColor("_SpecColor", NewColor);
                rendhandle.material = mat;
                //
                mat = new Material(rendPin1.sharedMaterial);
                mat.SetColor("_SpecColor", NewColor);
                rendPin1.material = mat;
                //
                mat = new Material(rendPin2.sharedMaterial);
                mat.SetColor("_SpecColor", NewColor);
                rendPin2.material = mat;
                //
                mat = new Material(rendCenter.sharedMaterial);
                mat.SetColor("_SpecColor", NewColor);
                rendCenter.material = mat;
//                break;

//            case "Clock1":

//                break;
//        }
        

    }
   
    //Set a new Date and time for CustomClock.
    public void SetNewDateTime(int Year, int Month, int Day, int Hour, int Minute, int Second)
    {
        SystemDate = true;
        CustomClock = new System.DateTime(Year, Month, Day, Hour, Minute, Second);
        SystemDate = false;
    }
    void Update()
    {
        SystemClock();
        if (ClockType == "Clock3")
        {
            VerifyAlarm();
            Alarm = new System.DateTime(1, 1, 1, AlarmHour, AlarmMin, 0);
            sAlarmM = Alarm.Minute;
            sAlarmH = Alarm.Hour % 12;
        }
        
    }
}

