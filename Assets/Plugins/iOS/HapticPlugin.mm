#import <UIKit/UIKit.h>

static UIImpactFeedbackGenerator *lightGenerator = nil;
static UIImpactFeedbackGenerator *mediumGenerator = nil;
static UIImpactFeedbackGenerator *heavyGenerator = nil;
static UINotificationFeedbackGenerator *notificationGenerator = nil;
static UISelectionFeedbackGenerator *selectionGenerator = nil;

extern "C" {

void _HapticPrepare() {
    if (lightGenerator == nil) {
        lightGenerator = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
        mediumGenerator = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
        heavyGenerator = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
        notificationGenerator = [[UINotificationFeedbackGenerator alloc] init];
        selectionGenerator = [[UISelectionFeedbackGenerator alloc] init];
    }
    [lightGenerator prepare];
    [mediumGenerator prepare];
    [heavyGenerator prepare];
    [notificationGenerator prepare];
    [selectionGenerator prepare];
}

void _HapticImpactLight() {
    if (lightGenerator == nil) _HapticPrepare();
    [lightGenerator impactOccurred];
}

void _HapticImpactMedium() {
    if (mediumGenerator == nil) _HapticPrepare();
    [mediumGenerator impactOccurred];
}

void _HapticImpactHeavy() {
    if (heavyGenerator == nil) _HapticPrepare();
    [heavyGenerator impactOccurred];
}

void _HapticNotificationSuccess() {
    if (notificationGenerator == nil) _HapticPrepare();
    [notificationGenerator notificationOccurred:UINotificationFeedbackTypeSuccess];
}

void _HapticNotificationWarning() {
    if (notificationGenerator == nil) _HapticPrepare();
    [notificationGenerator notificationOccurred:UINotificationFeedbackTypeWarning];
}

void _HapticNotificationError() {
    if (notificationGenerator == nil) _HapticPrepare();
    [notificationGenerator notificationOccurred:UINotificationFeedbackTypeError];
}

void _HapticSelection() {
    if (selectionGenerator == nil) _HapticPrepare();
    [selectionGenerator selectionChanged];
}

}
