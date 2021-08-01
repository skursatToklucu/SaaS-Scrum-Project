


    DOM = {
        passwForm: '.password-strength',
        passwErrorMsg: '.password-strength__error',
        passwInput1: document.querySelector('.password-strength__inputSettings1'),
        passwInput2: document.querySelector('.password-strength__inputSettings2'),
        passwVisibilityBtnSettings1: '.password-strength__visibilitySettings1',
        passwVisibilityBtnSettings2: '.password-strength__visibilitySettings2',
        passwVisibility_iconSettings: '.password-strength__visibility-iconSettings',
        strengthBar: document.querySelector('.password-strength__bar'),
        submitBtn: document.querySelector('.password-strength__submit'),
}


    //*** HELPERS

    //need to append classname with '.' symbol
    const findParentNode = (elem, parentClass) => {

        parentClass = parentClass.slice(1, parentClass.length);

        while (true) {

            if (!elem.classList.contains(parentClass)) {
        elem = elem.parentNode;
            } else {
                return elem;
            }

        }

    };

    //*** MAIN CODE

    const getPasswordVal = input => {
        return input.value;
    };

    const testPasswRegexp = (passw, regexp) => {

        return regexp.test(passw);

    };

    const testPassw = passw => {

        let strength = 'none';

        const moderate = /(?=.*[A-Z])(?=.*[a-z])(?=.*[\d]).{6,}/g;
        const strong = /(?=.*[A-Z])(?=.*[a-z])(?=.*[\d])(?=.*[,.]).{7,}/g;
        const extraStrong = /(?=.*[A-Z])(?=.*[a-z])(?=.*[\d])(?=.*[,.]).{9,}/g;

        if (testPasswRegexp(passw, extraStrong)) {
        strength = 'extra';
        } else if (testPasswRegexp(passw, strong)) {
        strength = 'strong';
        } else if (testPasswRegexp(passw, moderate)) {
        strength = 'moderate';
        } else if (passw.length > 0) {
        strength = 'weak';
        }

        return strength;

    };

    const testPasswError = passw => {

        const errorSymbols = /\s/g;

        return testPasswRegexp(passw, errorSymbols);

    };

    const setStrengthBarValue = (bar, strength) => {

        let strengthValue;

        switch (strength) {
            case 'weak':
                strengthValue = 25;
                bar.setAttribute('aria-valuenow', strengthValue);
                break;

            case 'moderate':
                strengthValue = 50;
                bar.setAttribute('aria-valuenow', strengthValue);
                break;

            case 'strong':
                strengthValue = 75;
                bar.setAttribute('aria-valuenow', strengthValue);
                break;

            case 'extra':
                strengthValue = 100;
                bar.setAttribute('aria-valuenow', strengthValue);
                break;

            default:
                strengthValue = 0;
                bar.setAttribute('aria-valuenow', 0);
        }

        return strengthValue;

    };

    //also adds a text label based on styles
    const setStrengthBarStyles = (bar, strengthValue) => {

        bar.style.width = `${strengthValue}%`;

        bar.classList.remove('bg-success', 'bg-info', 'bg-warning');

        switch (strengthValue) {
            case 25:
                bar.classList.add('bg-danger');
                bar.textContent = 'Çok zayıf';
                break;

            case 50:
                bar.classList.remove('bg-danger');
                bar.classList.add('bg-warning');
                bar.textContent = 'Zayıf';
                break;

            case 75:
                bar.classList.remove('bg-danger');
                bar.classList.add('bg-info');
                bar.textContent = 'Güçlü';
                break;

            case 100:
                bar.classList.remove('bg-danger');
                bar.classList.add('bg-success');
                bar.textContent = 'Çok Güçlü';
                break;

            default:
                bar.classList.add('bg-danger');
                bar.textContent = '';
                bar.style.width = `0`;
        }

    };

    const setStrengthBar = (bar, strength) => {

        //setting value
        const strengthValue = setStrengthBarValue(bar, strength);

       /* //setting styles*/
        setStrengthBarStyles(bar, strengthValue);
    };

    const unblockSubmitBtn = (btn, strength) => {

        if (strength === 'none' || strength === 'weak') {
        btn.disabled = true;
        } else {
        btn.disabled = false;
        }

    };
    const findErrorMsg = input => {
        const passwForm = findParentNode(input, DOM.passwForm);
        return passwForm.querySelector(DOM.passwErrorMsg);
    };

    const showErrorMsg = input => {
        const errorMsg = findErrorMsg(input);
        errorMsg.classList.remove('js-hidden');
    };

    const hideErrorMsg = input => {
        const errorMsg = findErrorMsg(input);
        errorMsg.classList.add('js-hidden');
    };

    const passwordStrength = (input, strengthBar, btn) => {

        //getting password
        const passw = getPasswordVal(input);

        //check if there is an error
        const error = testPasswError(passw);

        if (error) {

        showErrorMsg(input);

        } else {

        //hide error messages
        hideErrorMsg(input);

            //finding strength
            const strength = testPassw(passw);

            //setting strength bar (value and styles)
            setStrengthBar(strengthBar, strength);

            //unblock submit btn only if password is moderate or stronger
            unblockSubmitBtn(btn, strength);
        }

    };

    const passwordVisible = passwField => {

        const passwType = passwField.getAttribute('type');

        let visibilityStatus;

        if (passwType === 'text') {

        passwField.setAttribute('type', 'password');

            visibilityStatus = 'hidden';

        } else {

        passwField.setAttribute('type', 'text');

            visibilityStatus = 'visible';

        }

        return visibilityStatus;

    };

    const changeVisibiltyBtnIcon = (btn, status) => {

        const hiddenPasswIcon = btn.querySelector(`${DOM.passwVisibility_icon}[data-visible="hidden"]`);

        const visibilePasswIcon = btn.querySelector(`${DOM.passwVisibility_icon}[data-visible="visible"]`);

        if (status === 'visible') {
        visibilePasswIcon.classList.remove('js-hidden');
            hiddenPasswIcon.classList.add('js-hidden');
        } else if (status === 'hidden') {
        visibilePasswIcon.classList.add('js-hidden');
            hiddenPasswIcon.classList.remove('js-hidden');
        }

    };

    const passwVisibilitySwitcher = (passwField, visibilityToggler) => {

        const visibilityStatus = passwordVisible(passwField);

        changeVisibiltyBtnIcon(visibilityToggler, visibilityStatus);
    }

    //*** EVENT LISTENERS
DOM.passwInput1.addEventListener('input', () => {
    console.log("triggered");
        passwordStrength(DOM.passwInput1, DOM.strengthBar, DOM.submitBtn);
    });


const passwVisibilityBtn1 = document.querySelector(DOM.passwVisibilityBtnSettings1);

    passwVisibilityBtn1.addEventListener('click', e => {

        let toggler = findParentNode(e.target, DOM.passwVisibilityBtnSettings1);

        passwVisibilitySwitcher(DOM.passwInput1, toggler);

    });

const passwVisibilityBtn2 = document.querySelector(DOM.passwVisibilityBtnSettings2);

    passwVisibilityBtn2.addEventListener('click', e => {

        let toggler = findParentNode(e.target, DOM.passwVisibilityBtnSettings2);

        passwVisibilitySwitcher(DOM.passwInput2, toggler);

    });


 