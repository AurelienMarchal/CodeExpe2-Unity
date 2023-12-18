using UnityEngine;
using System.IO;

public class ResumeLogger
{
    const string columnSignature = (
        "user_ID;"+ 
        "group_ID;" + 
        "block_ID;" + 
        "visu;" + 
        "ti;" + 
        "task;" + 
        "trial_number;" + 
        "is_training;" +
        "number_of_object;" +
        "start_pos;" + 
        "target_pos;" + 
        "distance;" + 
        "left_Right;" + 
        "number_of_pattern_to_find;" + 
        "pattern_type;" +
        "number_of_pattern_found;" +
        "number_of_pattern_not_found;" +
        "did_find_all_patterns;" +
        "was_timed_out;" +
        "pattern1_pos;" +
        "pattern2_pos;" +
        "pattern3_pos;" +
        "pattern4_pos;" +
        "pattern5_pos;" +
        "first_pattern_found_pos;" +
        "second_pattern_found_pos;" +
        "third_pattern_found_pos;" +
        "fourth_pattern_found_pos;" +
        "fifth_pattern_found_pos;" +
        "total_time;" + 
        "time_before_first_move;" + 
        "time_grab;" + 
        "time_for_final_selection;" + 
        "time_to_validate_pattern1;" + 
        "time_to_validate_pattern2;" + 
        "time_to_validate_pattern3;" + 
        "time_to_validate_pattern4;" +
        "time_to_validate_pattern5;" + 
        "time_after_last_selection;" + 
        "time_using_interaction;" + 
        "number_of_retrieve;" + 
        "number_of_bad_select;" + 
        "number_of_click_inside_scrollbar_handle;" + 
        "number_of_click_outside_scrollbar_handle;" +
        "number_of_click_on_plus;" +
        "number_of_click_on_minus;" +
        "number_of_click_on_a;" +
        "number_of_click_on_b;" +
        "number_of_click_on_both_button;" +
        "number_of_interaction_started;" +
        "max_head_amplitude;" + 
        "max_zoom;" +
        "max_controller_distance;" +
        "max_ortho_distance;" +
        "max_cd_gain;" +
        "zoom_grab;" +
        "zoom_for_final_selection;" +
        "zoom_to_validate_pattern1;" +
        "zoom_to_validate_pattern2;" +
        "zoom_to_validate_pattern3;" +
        "zoom_to_validate_pattern4;" +
        "zoom_to_validate_pattern5;" +
        "controller_distance_grab;" +
        "controller_distance_for_final_selection;" +
        "controller_distance_to_validate_pattern1;" +
        "controller_distance_to_validate_pattern2;" +
        "controller_distance_to_validate_pattern3;" +
        "controller_distance_to_validate_pattern4;" +
        "controller_distance_to_validate_pattern5;" 
    );

    string logFileName;

    string logFullPath;

    int lineNumber = 0;

    void CreateFile(int userID){
        string expeLogsDirectory = Application.persistentDataPath + "/expeLogs";
        string resumeLogsDirectory = expeLogsDirectory + "/resumeLogs";
        //C:\Users\amarchal\AppData\LocalLow\IRIT\ThreeDplusT_Expe\expeLogs\resumeLogs\
        //Ce PC\Quest Pro\Espace de stockage interne partagé\Android\data\com.IRIT.ThreeDplusT_Expe\files\expeLogs\resumeLogs\
        logFileName = $"resume_user{userID}.csv";
        logFullPath = resumeLogsDirectory + "/" + logFileName;
        if(!Directory.Exists(expeLogsDirectory)){
            Directory.CreateDirectory(expeLogsDirectory);
        }

        if(!Directory.Exists(resumeLogsDirectory)){
            Directory.CreateDirectory(resumeLogsDirectory);
        }


        if(!File.Exists(logFullPath)){
            File.Create(logFullPath).Dispose();
            
            using(var sw = new StreamWriter(logFullPath, true))
            {
                sw.WriteLine(columnSignature);
            }
        }
        else{
            lineNumber = File.ReadAllLines(logFullPath).Length - 1;
        }
    }

    private void WriteLogLine(int userID, string logLine){
        CreateFile(userID);
        //Debug.Log("lineNumber, " +  logLine);
        using(var sw = new StreamWriter(logFullPath, true)){
            sw.WriteLine(logLine);
        }
    }

    public void LogTrial(
        int user_ID, 
        int group_ID, 
        int block_ID,

        string visu,
        string ti,
        string task,

        int trial_number,
        bool is_training,
        int number_of_object,
        int start_pos,
        int? target_pos,
        int? distance,
        string left_Right,

        int? number_of_pattern_to_find,
        string pattern_type,
        int? number_of_pattern_found,
        int? number_of_pattern_not_found,
        bool? did_find_all_patterns,
        bool? was_timed_out,
        int? pattern1_pos,
        int? pattern2_pos,
        int? pattern3_pos,
        int? pattern4_pos,
        int? pattern5_pos,
        int? pattern1_found_pos,
        int? pattern2_found_pos,
        int? pattern3_found_pos,
        int? pattern4_found_pos,
        int? pattern5_found_pos,

        float total_time,

        float time_before_first_move,

        float? time_grab,
        float? time_for_final_selection,

        float? time_to_validate_pattern1,
        float? time_to_validate_pattern2,
        float? time_to_validate_pattern3,
        float? time_to_validate_pattern4,
        float? time_to_validate_pattern5,
        float? time_after_last_selection,

        float? time_using_interaction,

        int? number_of_retrieve,
        int number_of_bad_select,
        int? number_of_click_inside_scrollbar_handle,
        int? number_of_click_outside_scrollbar_handle,
        int? number_of_click_on_plus,
        int? number_of_click_on_minus,

        int? number_of_click_on_a,
        int? number_of_click_on_b,
        int? number_of_click_on_both_button,

        int number_of_interaction_started,

        float max_head_amplitude,
        float? max_zoom,
        float? max_controller_distance,
        float? max_ortho_distance,
        float? max_cd_gain,
        
        float? zoom_grab,
        float? zoom_for_final_selection,
        
        float? zoom_to_validate_pattern1,
        float? zoom_to_validate_pattern2,
        float? zoom_to_validate_pattern3,
        float? zoom_to_validate_pattern4,
        float? zoom_to_validate_pattern5,

        float? controller_distance_grab,
        float? controller_distance_for_final_selection,

        float? controller_distance_to_validate_pattern1,
        float? controller_distance_to_validate_pattern2,
        float? controller_distance_to_validate_pattern3,
        float? controller_distance_to_validate_pattern4,
        float? controller_distance_to_validate_pattern5

        ){
        
        WriteLogLine(
            user_ID,
            $"P{user_ID};"+ 
            $"{group_ID};" + 
            $"{block_ID};" + 
            $"{visu};" + 
            $"{ti};" + 
            $"{task};" + 
            $"{trial_number};" + 
            $"{is_training};" + 
            $"{number_of_object};" +
            $"{start_pos};" + 
            $"{target_pos};" + 
            $"{distance};" + 
            $"{left_Right};" + 
            $"{number_of_pattern_to_find};" + 
            $"{pattern_type};" +
            $"{number_of_pattern_found};" +
            $"{number_of_pattern_not_found};" +
            $"{did_find_all_patterns};" +
            $"{was_timed_out};" +
            $"{pattern1_pos};" +
            $"{pattern2_pos};" +
            $"{pattern3_pos};" +
            $"{pattern4_pos};" +
            $"{pattern5_pos};" +
            $"{pattern1_found_pos};" +
            $"{pattern2_found_pos};" +
            $"{pattern3_found_pos};" +
            $"{pattern4_found_pos};" +
            $"{pattern5_found_pos};" +
            $"{total_time};" + 
            $"{time_before_first_move};" + 
            $"{time_grab};" + 
            $"{time_for_final_selection};" + 
            $"{time_to_validate_pattern1};" + 
            $"{time_to_validate_pattern2};" + 
            $"{time_to_validate_pattern3};" + 
            $"{time_to_validate_pattern4};" +
            $"{time_to_validate_pattern5};" + 
            $"{time_after_last_selection};" + 
            $"{time_using_interaction};" + 
            $"{number_of_retrieve};" + 
            $"{number_of_bad_select};" + 
            $"{number_of_click_inside_scrollbar_handle};" + 
            $"{number_of_click_outside_scrollbar_handle};" +
            $"{number_of_click_on_plus};" +
            $"{number_of_click_on_minus};" +
            $"{number_of_click_on_a};" +
            $"{number_of_click_on_b};" +
            $"{number_of_click_on_both_button};" +
            $"{number_of_interaction_started};" +
            $"{max_head_amplitude};" + 
            $"{max_zoom};" +
            $"{max_controller_distance};" +
            $"{max_ortho_distance};" +
            $"{max_cd_gain};" +
            $"{zoom_grab};" +
            $"{zoom_for_final_selection};" +
            $"{zoom_to_validate_pattern1};" +
            $"{zoom_to_validate_pattern2};" +
            $"{zoom_to_validate_pattern3};" +
            $"{zoom_to_validate_pattern4};" +
            $"{zoom_to_validate_pattern5};" +
            $"{controller_distance_grab};" +
            $"{controller_distance_for_final_selection};" +
            $"{controller_distance_to_validate_pattern1};" +
            $"{controller_distance_to_validate_pattern2};" +
            $"{controller_distance_to_validate_pattern3};" +
            $"{controller_distance_to_validate_pattern4};" +
            $"{controller_distance_to_validate_pattern5};" 
        );
    }
}
