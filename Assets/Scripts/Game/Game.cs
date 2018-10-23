using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	public List<GameObject> Goals;
	public Text ScoreText;
	public Text WowText;
	public Button PlayPauseButton;
	public Button NextLevelButton;
	public GameObject WinnerPanel;
	public GameObject ExplosionPrefab;
	public GameObject FireworksPrefab;
	private bool isPlaying = false;
	private Rigidbody ballRigidbody;
	private Vector3 initialBallPosition;
	private int score = 0;
	private int winScore;

	// Use this for initialization
	void Start () {
		ballRigidbody = GetComponent<Rigidbody>();
		winScore = Goals.Count;
		PlayPauseButton.onClick.AddListener(TogglePlayMode);
		NextLevelButton.onClick.AddListener(NextLevel);
		initialBallPosition = new Vector3(
			gameObject.transform.position.x,
			gameObject.transform.position.y,
			gameObject.transform.position.z
		);
		WinnerPanel.SetActive(false);
		NextLevelButton.gameObject.SetActive(true);
		UpdateScore();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void TogglePlayMode() {
		isPlaying = !isPlaying;

		if (isPlaying) {
			ballRigidbody.useGravity = true;
			var explosions = GameObject.FindGameObjectsWithTag("Explosion");
			foreach (var explosion in explosions) {
				Destroy(explosion);
			}
		}
		else {
			ballRigidbody.useGravity = false;
			ballRigidbody.velocity = Vector3.zero;
			CleanGame();
		}
	}

	void CleanGame() {
		// Reset Score
		score = 0;
		UpdateScore();

		// Reset Ball
		gameObject.transform.position = initialBallPosition;

		// Reset Drawings
        var lines = GameObject.FindGameObjectsWithTag("Line");
		foreach (var line in lines) {
			Destroy(line);
		}

		// Reset Goals
		var goals = GameObject.FindGameObjectsWithTag("Goal");
		foreach (var goal in Goals) {
			goal.SetActive(true);
		}

		isPlaying = false;
    }


	void Winner() {
		ballRigidbody.velocity = Vector3.zero;
		ballRigidbody.useGravity = false;
		var fireworks = Instantiate(FireworksPrefab, new Vector3(0, 0, 1), Quaternion.Euler(-90, 0, 0));
		WinnerPanel.SetActive(true);
	}


	void NextLevel() {
		var scene = SceneManager.GetActiveScene();
		var lastChar = scene.name[scene.name.Length - 1].ToString();

        if (lastChar == "0")
        {
            WowText.text = "You beat the game!";
            NextLevelButton.gameObject.SetActive(false);
        }
        else
        {
            var nextScene = "Level" + (int.Parse(lastChar) + 1).ToString();
            SceneManager.LoadScene(nextScene);
        }
	}


	void UpdateScore() {
		ScoreText.text = "Score: " + score.ToString() + "/" + winScore.ToString();

		// Check for win
		if (score >= winScore) {
			Winner();
		}
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.CompareTag("Goal")) {
			collider.gameObject.SetActive(false);
			score++;
			UpdateScore();
		} 
		else if (collider.gameObject.CompareTag("Death")) {
			var explosion = Instantiate(ExplosionPrefab, gameObject.transform.position, Quaternion.identity);
			explosion.gameObject.tag = "Explosion";
			TogglePlayMode();
		}

	}
}
