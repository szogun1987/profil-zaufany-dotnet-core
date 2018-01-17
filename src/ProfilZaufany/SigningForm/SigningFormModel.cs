﻿namespace ProfilZaufany.SigningForm
{
    /// <summary>
    /// Model formularza logowania
    /// </summary>
    public class SigningFormModel
    {
        /// <summary>
        /// Wartość pola &quot;action&quot; elementu form
        /// </summary>
        public string FormAction { get; set; }

        /// <summary>
        /// Wartość ukrtytego pola formularza SAMLRequest
        /// </summary>
        public string SAMLRequest { get; set; }
    }
}