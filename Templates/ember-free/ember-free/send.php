<?php
// send.php — simple, dependency-free contact handler for the landing page templates.
// Configure the recipient below, upload alongside the HTML files on any PHP host.

$TO      = "you@yourdomain.com";        // <-- CHANGE THIS to your email
$SUBJECT = "New website enquiry";

// Only accept POST
if ($_SERVER["REQUEST_METHOD"] !== "POST") {
    http_response_code(405);
    exit("Method not allowed");
}

// Grab + sanitize
function clean($k){ return isset($_POST[$k]) ? trim(filter_var($_POST[$k], FILTER_UNSAFE_RAW)) : ""; }
$name    = clean("name");
$email   = clean("email");
$phone   = clean("phone");
$service = clean("service");
$message = clean("message");
$source  = clean("_source");

// Basic validation
$errors = [];
if ($name === "")  $errors[] = "name";
if ($phone === "") $errors[] = "phone";
if (!filter_var($email, FILTER_VALIDATE_EMAIL)) $errors[] = "email";
if ($message === "") $errors[] = "message";

if ($errors) {
    http_response_code(422);
    exit("Please complete: " . implode(", ", $errors));
}

// Build email
$body  = "New enquiry from your website ($source)\n\n";
$body .= "Name:    $name\n";
$body .= "Email:   $email\n";
$body .= "Phone:   $phone\n";
$body .= "Service: $service\n\n";
$body .= "Message:\n$message\n";

$headers  = "From: website@" . ($_SERVER['SERVER_NAME'] ?? 'localhost') . "\r\n";
$headers .= "Reply-To: $email\r\n";
$headers .= "Content-Type: text/plain; charset=UTF-8\r\n";

// Send (uses PHP mail(); swap for SMTP/PHPMailer in production if needed)
$ok = @mail($TO, $SUBJECT, $body, $headers);

if ($ok) {
    http_response_code(200);
    echo "OK";
} else {
    http_response_code(500);
    echo "Mail failed";
}
