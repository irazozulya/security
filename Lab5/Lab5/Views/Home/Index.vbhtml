@Code
    ViewData("Title") = "Home Page"
End Code

<div class="row">
	<div class="container">
		<label for="uname"><b>Username</b></label>
		<input type="text" placeholder="Enter Username" name="uname" required>
	</div>
	<div class="container">
		<label for="psw"><b>Password</b></label>
		<input type="password" placeholder="Enter Password" name="psw" required>
	</div>
	<div class="container">
		<button type="submit">Login</button>
	</div>
</div>
